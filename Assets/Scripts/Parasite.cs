using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using MonstersDomain.BehaviorTree;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

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
        [SerializeField] VisionSensor _visionSensor;
        [SerializeField] GameObject _deathTimeline;
        [SerializeField] HearingSensor _hearingSensor;

        [SerializeField] [Tooltip("目標を見失うまでの時間")]
        float _timeUntilLoseSightOfTarget = 3;

        [SerializeField] [Tooltip("目標を見失う距離")] float _distanceLoseSightOfTarget = 20;
        NavMeshAgent _agent;
        Root _behaviorRoot;
        Coroutine _chaseCoroutine, _patrolCoroutine;
        Player _currentTarget;
        float _lostTargetTimer = float.MaxValue;
        int _patrolIndex;
        readonly HashSet<Player> _playerHashSet = new();
        readonly ReactiveProperty<ParasiteState> _state = new(ParasiteState.Patrol);
        ObservableStateMachineTrigger _trigger;
        bool _isPaused = false;

        void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _state.SkipLatestValueOnSubscribe().Where(s => s == ParasiteState.Patrol)
                .Subscribe(_ =>
                {
                    if (_patrolCoroutine != null) StopCoroutine(_patrolCoroutine);
                    _patrolCoroutine = null;
                    if (_chaseCoroutine != null) StopCoroutine(_chaseCoroutine);
                    _chaseCoroutine = null;
                    _currentTarget = null;
                    _patrolCoroutine = StartCoroutine(Patrol());
                    AudioManager.Instance.StopMusic();
                    AudioManager.Instance.StopAmbient();
                    GameManager.Instance.ChasePostEffect(false);
                    _agent.speed = _patrolSpeed;
                }).AddTo(this);
            _state.SkipLatestValueOnSubscribe().Where(s => s == ParasiteState.Chase)
                .Subscribe(_ => StartChase()).AddTo(this);
            _state.SkipLatestValueOnSubscribe().Where(s => s == ParasiteState.Check)
                .Subscribe(_ =>
                {
                    if (_patrolCoroutine != null) StopCoroutine(_patrolCoroutine);
                    _patrolCoroutine = null;
                }).AddTo(this);
            // AnimatorからObservableStateMachineTriggerの参照を取得
            _trigger = _animator.GetBehaviour<ObservableStateMachineTrigger>();
            _eventDispatcher.EventFootSteps.Subscribe(_ =>
            {
                AudioManager.Instance.Play3DFootSteps(FootSteps.Parasite, transform.position);
                _footStepsImpulseSource.GenerateImpulse();
            });
            _behaviorRoot = BT.Root(new List<Node>
            {
                BT.Sequence(new List<Node>
                {
                    BT.ParallelSelector(new List<Node>() //  索敵、プレイヤーを見つけたら次のノード
                    {
                        BT.Condition(() => CheckTargetBySight()),
                        BT.Sequence(new List<Node>
                        {
                            BT.Inverter(BT.If(()=>_hearingSensor.CheckLocation != Vector3.zero).OpenBranch(new()
                            {
                                BT.Action(() =>
                                {
                                    AudioManager.Instance.Play3DSE(SE.Bite, transform.position);
                                    _state.Value = ParasiteState.Check;
                                    _agent.SetDestination(_hearingSensor.CheckLocation);
                                    _hearingSensor.CheckLocation = Vector3.zero;
                                })
                            })),
                            BT.Condition(()=> !_agent.hasPath || _state.Value != ParasiteState.Check),
                            BT.Action(() => _state.Value = ParasiteState.Patrol), //  パトロール状態に入る
                            BT.Inverter(BT.Wait(1))
                        })
                    }),
                    BT.Selector(new List<Node>
                    {
                        BT.Sequence(new List<Node>
                        {
                            BT.Inverter(BT.Condition(() => CheckTargetByDistance())), //  距離が離れていれば
                            BT.Action(() => _state.Value = ParasiteState.Patrol), //  パトロール状態に入る
                            BT.Wait(1)
                        }),
                        BT.Sequence(new List<Node>
                        {
                            BT.Action(() => _state.Value = ParasiteState.Chase),
                            BT.Wait(1)
                        })
                    })
                })
            });
            _patrolCoroutine ??= StartCoroutine(Patrol());
            GameManager.Instance.OnPause += OnPause;
            GameManager.Instance.OnResume += OnResume;
        }

        void OnDisable()
        {
            GameManager.Instance.OnPause -= OnPause;
            GameManager.Instance.OnResume -= OnResume;
        }

        void OnPause()
        {
            _isPaused = true;
            _agent.Stop();
        }

        void OnResume()
        {
            _isPaused = false;
            _agent.Resume();
        }

        void Update()
        {
            if (_isPaused) return;
            
            _behaviorRoot.Tick();
            if (_lostTargetTimer < _timeUntilLoseSightOfTarget)
                _lostTargetTimer += Time.deltaTime;
            if (_playerHashSet.Count > 0)
            {
                var player = _playerHashSet.First();
                if (!player.IsDied && player.State.Value != PlayerState.Hide) StartCoroutine(EatingTarget(player));
            }
        }

        void LateUpdate()
        {
            if (_animator && _agent && _agent.velocity.magnitude > 0)
                _animator.SetFloat("Speed", _agent.speed);
            else
                _animator.SetFloat("Speed", 0);
        }
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (_agent != null)
            {
                Gizmos.color = Color.red;
                var prefPos = transform.position;
                foreach (var p in _agent.path.corners)
                {
                    Gizmos.DrawLine(prefPos, p);
                    prefPos = p;
                }
            }
        }
#endif

        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player)) _playerHashSet.Add(player);
        }

        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player player)) _playerHashSet.Remove(player);
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
            var sqrDistance = (transform.position - _currentTarget.transform.position).sqrMagnitude;
            return sqrDistance < _distanceLoseSightOfTarget * _distanceLoseSightOfTarget;
        }

        IEnumerator EatingTarget(Player player)
        {
            player.IsDied = true;
            var playerPos = player.transform.position;
            //  死亡演出
            //  敵のレイヤーをカメラのマスクから除外する
            Camera.main.cullingMask &= ~(1 << 10);
            AudioManager.Instance.PlaySE(SE.JumpScare);
            var main = Camera.main.GetComponent<CinemachineBrain>();
            var playable = Instantiate(_deathTimeline, transform.position, Quaternion.identity)
                .GetComponent<PlayableDirector>();
            var playablePos = playable.transform.position;
            playable.gameObject.transform.forward =
                (new Vector3(playerPos.x, playablePos.y, playerPos.z) - playablePos).normalized;
            var binding = playable.playableAsset.outputs.First(c => c.streamName == "Cinemachine Track");
            playable.SetGenericBinding(binding.sourceObject, main);
            yield return new WaitForSeconds(1);
            GameManager.Instance.CurrentGameState.Value = GameState.GameOver;
        }

        void StartChase()
        {
            if (_patrolCoroutine != null) StopCoroutine(_patrolCoroutine);
            _patrolCoroutine = null;
            if (_chaseCoroutine != null) StopCoroutine(_chaseCoroutine);
            _chaseCoroutine = null;
            _agent.isStopped = true;
            AudioManager.Instance.PlaySE(SE.Roar);
            AudioManager.Instance.PlayAmbient(Ambient.Chase);
            GameManager.Instance.ChasePostEffect(true);
            _roarCinemachineImpulseSource.GenerateImpulse();
            _trigger.OnStateExitAsObservable()
                .Where(i => i.StateInfo.IsName("Base Layer.Mutant Roaring"))
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
                if (!_isPaused)
                    if (_currentTarget) _agent.SetDestination(_currentTarget.transform.position);
                yield return null;
            }
        }

        IEnumerator Patrol()
        {
            var wait = new WaitForSeconds(1);
            _agent.SetDestination(_patrolAnchor[_patrolIndex].position);
            while (true)
            {
                if (!_isPaused)
                {
                    if (!_agent.hasPath)
                    {
                        _agent.SetDestination(_patrolAnchor[_patrolIndex].position);
                        _patrolIndex++;
                        _patrolIndex %= _patrolAnchor.Length;
                    }
                }
                yield return wait;
            }
        }
    }

    public enum ParasiteState
    {
        Patrol,
        Chase,
        Check
    }
}