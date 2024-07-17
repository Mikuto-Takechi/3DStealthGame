using System;
using UnityEngine;
using UnityEngine.AI;

namespace MonstersDomain
{
    [Serializable]
    public class MoveController : MonoBehaviour
    {
        [SerializeField] NavMeshAgent _agent;
        PatrolAsyncMover _patrolAsyncMover;
        //  TODO :グラフの方でタイマー処理を書けなかったので一時的にMoveControllerにタイマーを持たせている。
        float _chaseTimer;
        public NavMeshAgent Agent => _agent;
        public PointAsyncMover PointAsyncMover { get; private set; }

        public ChaseAsyncMover ChaseAsyncMover { get; private set; }
        public float ChaseTimer
        {
            get => _chaseTimer;
            set => _chaseTimer = value;
        }

        void Awake()
        {
            ChaseAsyncMover = GetComponent<ChaseAsyncMover>();
            _patrolAsyncMover = GetComponent<PatrolAsyncMover>();
            PointAsyncMover = GetComponent<PointAsyncMover>();
        }

        void Update()
        {
            if (_chaseTimer > 0)
            {
                _chaseTimer -= Time.deltaTime;
            }
        }

        public void Chase(Transform target)
        {
            _patrolAsyncMover?.StopMove();
            PointAsyncMover?.StopMove();
            ChaseAsyncMover?.StartChase(_agent, target);
        }

        public void Patrol()
        {
            PointAsyncMover?.StopMove();
            ChaseAsyncMover?.StopMove();
            _patrolAsyncMover?.StartPatrol(_agent);
        }

        public void MoveToPoint(Vector3 point)
        {
            _patrolAsyncMover?.StopMove();
            ChaseAsyncMover?.StopMove();
            PointAsyncMover?.StartMoveToPoint(_agent, point);
        }
    }
}