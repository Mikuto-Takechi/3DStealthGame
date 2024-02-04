using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;
using System.Linq;
using MonstersDomain.BehaviorTree;
using Action = System.Action;

namespace MonstersDomain
{
    public class Parasite : MonoBehaviour
    {
        [SerializeField] float _chaseSpeed = 4f;
        [SerializeField] float _patrolSpeed = 2f;
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
        Player _currentTarget;
        int _patrolIndex = 0;
        NavMeshAgent _agent;
        ReactiveProperty<ParasiteState> _state = new(ParasiteState.Patrol);
        Root _behaviorRoot;
        float _lostTargetTimer = float.MaxValue;
        [SerializeField, Tooltip("目標を見失うまでの時間")] float _timeUntilLoseSightOfTarget = 3;
        [SerializeField, Tooltip("目標を見失う距離")] float _distanceLoseSightOfTarget = 20;
        Coroutine _chaseCoroutine, _patrolCoroutine;
        HashSet<Player> _playerHashSet = new();
        void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _state.SkipLatestValueOnSubscribe().Where(s => s == ParasiteState.Patrol)
                .Subscribe(_ =>
                {
                    if(_chaseCoroutine != null) StopCoroutine(_chaseCoroutine);
                    _chaseCoroutine = null;
                    _currentTarget = null;
                    _patrolCoroutine = StartCoroutine(Patrol());
                    AudioManager.Instance.StopMusic();
                    AudioManager.Instance.StopAmbient();
                    _chromatic.enabled.value = false;
                    _vignette.enabled.value = false;
                    _agent.speed = _patrolSpeed;
                }).AddTo(this);
            _state.SkipLatestValueOnSubscribe().Where(s=> s == ParasiteState.Chase)
                .Subscribe(_=> StartChase()).AddTo(this);
            // AnimatorからObservableStateMachineTriggerの参照を取得
            _trigger = _animator.GetBehaviour<ObservableStateMachineTrigger>();
            _eventDispatcher.EventFootSteps.Subscribe(_=>
            {
                AudioManager.Instance.Play3DFootSteps(FootSteps.Parasite, transform.position);
                _footStepsImpulseSource.GenerateImpulse();
            });
            _volume.profile.TryGetSettings(out _chromatic);
            _volume.profile.TryGetSettings(out _vignette);
            _behaviorRoot = BT.Root(new()
            {
                BT.Sequence(new()
                {
                    BT.ParallelSelector(new()   //  索敵、プレイヤーを見つけたら次のノード
                    {
                        BT.Condition(() => CheckTargetBySight()),
                        BT.Sequence(new()
                        {
                            BT.Action(()=> _state.Value = ParasiteState.Patrol),    //  パトロール状態に入る
                            BT.Inverter(BT.Wait(1))
                        })
                    }),
                    BT.Selector(new List<Node>
                    {
                        BT.Sequence(new()
                        {
                            BT.Inverter(BT.Condition(()=>CheckTargetByDistance())),    //  距離が離れていれば
                            BT.Action(()=> _state.Value = ParasiteState.Patrol),    //  パトロール状態に入る
                            BT.Wait(1)
                        }),
                        BT.Sequence(new()
                        {
                            BT.Action(()=>_state.Value = ParasiteState.Chase),
                            BT.Wait(1)
                        })
                    })
                })
            });
            _patrolCoroutine ??= StartCoroutine(Patrol());
        }

        bool CheckTargetBySight()
        {
            if (_visionSensor.InSightTarget(ref _currentTarget))
            {
                _lostTargetTimer = 0;
                return true;
            }
            return _lostTargetTimer < _timeUntilLoseSightOfTarget;
        }

        bool CheckTargetByDistance()
        {
            if (_currentTarget == null) return false;
            float sqrDistance = (transform.position - _currentTarget.transform.position).sqrMagnitude;
            return sqrDistance < _distanceLoseSightOfTarget * _distanceLoseSightOfTarget;
        }
        void Update()
        {
            _behaviorRoot.Tick();
            if (_lostTargetTimer < _timeUntilLoseSightOfTarget)
                _lostTargetTimer += Time.deltaTime;
            if (_playerHashSet.Count > 0)
            {
                var player = _playerHashSet.First();
                if (!player.IsDied && player.State.Value != PlayerState.Hide)
                {
                    StartCoroutine(EatingTarget(player));
                }
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
            if (other.TryGetComponent(out Player player))
            {
                _playerHashSet.Add(player);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                _playerHashSet.Remove(player);
            }
        }

        IEnumerator EatingTarget(Player player)
        {
            player.IsDied = true;
            Vector3 playerPos = player.transform.position;
            //  死亡演出
            //  敵のレイヤーをカメラのマスクから除外する
            Camera.main.cullingMask &= ~(1 << 10);
            AudioManager.Instance.PlaySE(SE.JumpScare);
            CinemachineBrain main = Camera.main.GetComponent<CinemachineBrain>();
            PlayableDirector playable = Instantiate(_deathTimeline, transform.position, Quaternion.identity).GetComponent<PlayableDirector>();
            Vector3 playablePos = playable.transform.position;
            playable.gameObject.transform.forward =
                (new Vector3(playerPos.x, playablePos.y, playerPos.z) - playablePos).normalized;
            var binding = playable.playableAsset.outputs.First(c => c.streamName == "Cinemachine Track");
            playable.SetGenericBinding(binding.sourceObject, main);
            yield return new WaitForSeconds(1);
            GameManager.Instance.CurrentGameState.Value = GameState.GameOver;
        }
        void StartChase()
        {
            if(_patrolCoroutine != null) StopCoroutine(_patrolCoroutine);
            _patrolCoroutine = null;
            _agent.isStopped = true;
            AudioManager.Instance.PlaySE(SE.Roar);
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
                    _chaseCoroutine = StartCoroutine(Chase());
                }).AddTo(this);
            _animator.SetTrigger("Roar");
        }
        IEnumerator Chase()
        {
            while (true)
            {
                if (_currentTarget)
                {
                    _agent.SetDestination(_currentTarget.transform.position);
                }
                yield return null; 
            }
        }
        
        IEnumerator Patrol()
        {
            _agent.SetDestination(_patrolAnchor[_patrolIndex].position);
            while (true)
            {
                if(!_agent.hasPath)
                {
                    _agent.SetDestination(_patrolAnchor[_patrolIndex].position);
                    _patrolIndex++;
                    _patrolIndex %= _patrolAnchor.Length;
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