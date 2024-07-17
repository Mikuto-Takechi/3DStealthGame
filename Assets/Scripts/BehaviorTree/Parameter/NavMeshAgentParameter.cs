using System;
using GraphProcessor;
using UnityEngine;
using UnityEngine.AI;

namespace MonstersDomain.BehaviorTree
{
    [Serializable]
    public class NavMeshAgentParameter : ExposedParameter
    {
        [SerializeField] NavMeshAgent val;

        public override object value
        {
            get => val;
            set => val = (NavMeshAgent)value;
        }

        public override Type GetValueType()
        {
            return typeof(NavMeshAgent);
        }
    }
}