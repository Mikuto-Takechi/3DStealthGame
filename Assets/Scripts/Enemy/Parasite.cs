using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

namespace MonstersDomain
{
    public class Parasite : MonoBehaviour
    {
        [SerializeField] BehaviorTreeGraph _behaviorTree;
        [SerializeField] float _chaseSpeed = 4f;
        [SerializeField] float _patrolSpeed = 2f;
        [SerializeField] Animator _animator;
        [SerializeField] ParasiteEventDispatcher _eventDispatcher;
        [SerializeField] CinemachineImpulseSource _footStepsImpulseSource;
        [SerializeField] CinemachineImpulseSource _roarCinemachineImpulseSource;
        [SerializeField] VisionSensor _visionSensor;
        [SerializeField] GameObject _deathTimeline;
        [SerializeField] HearingSensor _hearingSensor;
        [SerializeField] MoveController _moveController;
        readonly HashSet<Player> _playerHashSet = new();
        bool _isPaused;
        BehaviorTreeProcessor _treeProcessor;
        BehaviorTreeGraph _copyGraph;
        void Start()
        {
            //  グラフのインスタンスをコピーして使う
            _copyGraph = Instantiate(_behaviorTree);
            _copyGraph.SetParameterValue("Owner", gameObject);
            _copyGraph.SetParameterValue("VisionSensor", _visionSensor);
            _copyGraph.SetParameterValue("HearingSensor", _hearingSensor);
            _copyGraph.SetParameterValue("MoveController", _moveController);
            _copyGraph.SetParameterValue("Animator", _animator);
            _copyGraph.SetParameterValue("ImpulseSource", _roarCinemachineImpulseSource);
            _treeProcessor = new BehaviorTreeProcessor(_copyGraph);
            _treeProcessor.Run();
            //  プレイヤーの居るエリアが切り替わったらそのエリアのスポーン地点にワープする
            AreaManager.Instance.SwitchAreaSubject.Subscribe(warpPoint =>
            {
                if (_moveController.ChaseAsyncMover.Coroutine == null) _moveController.Agent.Warp(warpPoint);
            }).AddTo(this);
            _eventDispatcher.EventFootSteps.Subscribe(_ =>
            {
                AudioManager.Instance.Play3DFootSteps(FootSteps.Parasite, transform.position);
                _footStepsImpulseSource.GenerateImpulse();
            });
            _moveController.Patrol();
            _moveController.Agent.speed = _patrolSpeed;
            GameManager.Instance.OnPause += OnPause;
            GameManager.Instance.OnResume += OnResume;
        }

        void Update()
        {
            if (_isPaused) return;
            _treeProcessor.UpdateTick();
            if (_playerHashSet.Count > 0)
            {
                var player = _playerHashSet.First();
                if (!player.IsDied && player.State.Value != PlayerState.Hide) StartCoroutine(EatingTarget(player));
            }
        }

        void LateUpdate()
        {
            if (_animator && _moveController.Agent && _moveController.Agent.velocity.magnitude > 0)
                _animator.SetFloat("Speed", _moveController.Agent.speed);
            else
                _animator.SetFloat("Speed", 0);
        }

        void OnDisable()
        {
            GameManager.Instance.OnPause -= OnPause;
            GameManager.Instance.OnResume -= OnResume;
        }
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (_moveController.Agent != null)
            {
                Gizmos.color = Color.red;
                var prefPos = transform.position;
                foreach (var p in _moveController.Agent.path.corners)
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

        void OnPause()
        {
            _isPaused = true;
            _moveController.Agent.Stop();
        }

        void OnResume()
        {
            _isPaused = false;
            _moveController.Agent.Resume();
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
    }
}