using System;
using System.Collections;
using Cinemachine;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;
using System.Linq;

namespace MonstersDomain
{
    public class Parasite : MonoBehaviour
    {
        [SerializeField] float _chaseSpeed = 4f;
        [SerializeField] Transform[] _patrolAnchor;
        [SerializeField] Animator _animator;
        [SerializeField] ParasiteEventDispatcher _eventDispatcher;
        [SerializeField] CinemachineImpulseSource _footStepsImpulseSource;
        [SerializeField] CinemachineImpulseSource _roarCinemachineImpulseSource;
        [SerializeField] PostProcessVolume _volume;
        [SerializeField] VisionSensor _visionSensor;
        [SerializeField] GameObject _deathTimeline;
        ChromaticAberration _chromatic;
        Vignette _vignette;
        ObservableStateMachineTrigger _trigger;
        Player _chaseTarget;
        int _patrolIndex = 0;
        NavMeshAgent _agent;
        ReactiveProperty<ParasiteState> _state = new(ParasiteState.Patrol);
        void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _visionSensor.DetectionTarget.Subscribe(p =>
            {
                _chaseTarget = p;
                _state.Value = ParasiteState.Chase;
            });
            _state.SkipLatestValueOnSubscribe().Where(s=> s == ParasiteState.Chase)
                .Subscribe(_=> StartChase());
            // AnimatorからObservableStateMachineTriggerの参照を取得
            _trigger = _animator.GetBehaviour<ObservableStateMachineTrigger>();
            _eventDispatcher.EventFootSteps.Subscribe(_=>
            {
                AudioManager.Instance.Play3DFootSteps(FootSteps.Parasite, transform.position);
                _footStepsImpulseSource.GenerateImpulse();
            });
            _volume.profile.TryGetSettings(out _chromatic);
            _volume.profile.TryGetSettings(out _vignette);
        }
        void Update()
        {
            if (!_agent.hasPath && _state.Value == ParasiteState.Patrol)
            {
                Patrol();
            }
        }
        void LateUpdate()
        {
            if(_animator && _agent && _agent.velocity.magnitude > 0)
            {
                _animator.SetFloat("Speed", _agent.speed);
            }
            else
            {
                _animator.SetFloat("Speed", 0);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player) && !player.IsDied)
            {
                player.IsDied = true;
                Vector3 playerPos = player.transform.position;
                //  死亡演出
                CinemachineBrain main = Camera.main.GetComponent<CinemachineBrain>();
                PlayableDirector playable = Instantiate(_deathTimeline, transform.position, Quaternion.identity).GetComponent<PlayableDirector>();
                Vector3 playablePos = playable.transform.position;
                playable.gameObject.transform.forward =
                    (new Vector3(playerPos.x, playablePos.y, playerPos.z) - playablePos).normalized;
                var binding = playable.playableAsset.outputs.First(c => c.streamName == "Cinemachine Track");
                playable.SetGenericBinding(binding.sourceObject, main);
            }
        }

        void Patrol()
        {
            if(_patrolIndex >= _patrolAnchor.Length)
            {
                _patrolIndex = 0;
            }
            _agent.SetDestination(_patrolAnchor[_patrolIndex].position);
            _patrolIndex++;
        }
        void StartChase()
        {
            _agent.isStopped = true;
            AudioManager.Instance.PlaySE(SE.Roar);
            AudioManager.Instance.PlayMusic(Music.Chase);
            AudioManager.Instance.PlayAmbient(Ambient.Chase);
            _chromatic.enabled.value = true;
            _vignette.enabled.value = true;
            _roarCinemachineImpulseSource.GenerateImpulse();
            _trigger.OnStateExitAsObservable()
                .Where(i=> i.StateInfo.IsName("Base Layer.Mutant Roaring"))
                .First()
                .Subscribe(_ =>
                {
                    _agent.isStopped = false;
                    _agent.speed = _chaseSpeed;
                    StartCoroutine(Chase());
                }).AddTo(this);
            _animator.SetTrigger("Roar");
        }
        IEnumerator Chase()
        {
            while (true)
            {
                if (_chaseTarget)
                {
                    _agent.SetDestination(_chaseTarget.transform.position);
                }
                yield return null; 
            }
        }
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (_agent != null)
            {
                Gizmos.color = Color.red;
                Vector3 prefPos = transform.position;
                foreach (var p in _agent.path.corners)
                {
                    Gizmos.DrawLine(prefPos, p);
                    prefPos = p;
                }
            }
        }
#endif
    }

    public enum ParasiteState
    {
        Patrol,
        Chase,
    }
}