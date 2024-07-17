using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable]
    public class MoveControllerParameter : ExposedParameter
    {
        [SerializeField] MoveController val;

        public override object value
        {
            get => val;
            set => val = (MoveController)value;
        }

        public override Type GetValueType()
        {
            return typeof(MoveController);
        }
    }
}