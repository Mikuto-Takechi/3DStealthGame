using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace MonstersDomain
{
    public class ChaseAsyncMover : BaseAsyncMover
    {
        public void StartChase(NavMeshAgent agent, Transform target)
        {
            if (_coroutine != null) return;
            _coroutine = StartCoroutine(Chase(agent, target));
        }

        IEnumerator Chase(NavMeshAgent agent, Transform target)
        {
            while (true)
            {
                if (!_isPaused) agent.SetDestination(target.position);
                yield return null;
            }
        }
    }
}