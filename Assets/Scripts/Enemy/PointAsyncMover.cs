using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace MonstersDomain
{
    public class PointAsyncMover : BaseAsyncMover
    {
        public void StartMoveToPoint(NavMeshAgent agent, Vector3 point)
        {
            if (_coroutine != null) return;
            _coroutine = StartCoroutine(MoveToPoint(agent, point));
            Current = MoveState.Running;
        }

        IEnumerator MoveToPoint(NavMeshAgent agent, Vector3 point)
        {
            agent.SetDestination(point);
            while (true)
            {
                if (!_isPaused)
                    if (!agent.hasPath)
                    {
                        _coroutine = null;
                        Current = MoveState.Complete;
                        yield break;
                    }

                yield return null;
            }
        }
    }
}