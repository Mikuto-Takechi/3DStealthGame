using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable]
    public class AnimatorParameter : ExposedParameter
    {
        [SerializeField] Animator val;

        public override object value
        {
            get => val;
            set => val = (Animator)value;
        }

        public override Type GetValueType()
        {
            return typeof(Animator);
        }
    }
}