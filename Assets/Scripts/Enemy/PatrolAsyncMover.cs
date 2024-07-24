using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace MonstersDomain.Enemy
{
    public class PatrolAsyncMover : BaseAsyncMover
    {
        public void StartPatrol(NavMeshAgent agent)
        {
            if (_coroutine != null) return;
            _coroutine = StartCoroutine(Patrol(agent));
        }

        IEnumerator Patrol(NavMeshAgent agent)
        {
            var wait = new WaitForSeconds(0.1f);
            agent.SetDestination(AreaManager.Instance.GetDestination());
            while (true)
            {
                if (!_isPaused)
                    if (!agent.hasPath)
                    {
                        agent.SetDestination(AreaManager.Instance.GetDestination());
                        AreaManager.Instance.NextPatrolIndex();
                    }

                yield return wait;
            }
        }
    }
}