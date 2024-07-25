using System;
using UnityEngine;
using UnityEngine.AI;

namespace MonstersDomain.Enemy
{
    /// <summary>
    /// 敵の移動を管理するクラス。
    /// </summary>
    [Serializable]
    public class MoveController : MonoBehaviour
    {
        [SerializeField] NavMeshAgent _agent;
        PatrolAsyncMover _patrolAsyncMover;
        public NavMeshAgent Agent => _agent;
        public PointAsyncMover PointAsyncMover { get; private set; }

        public ChaseAsyncMover ChaseAsyncMover { get; private set; }
        void Awake()
        {
            ChaseAsyncMover = GetComponent<ChaseAsyncMover>();
            _patrolAsyncMover = GetComponent<PatrolAsyncMover>();
            PointAsyncMover = GetComponent<PointAsyncMover>();
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