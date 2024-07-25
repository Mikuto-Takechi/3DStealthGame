using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable]
    public class HearingSensorParameter : ExposedParameter
    {
        [SerializeField] HearingSensor val;

        public override object value
        {
            get => val;
            set => val = (HearingSensor)value;
        }

        public override Type GetValueType()
        {
            return typeof(HearingSensor);
        }
    }
}