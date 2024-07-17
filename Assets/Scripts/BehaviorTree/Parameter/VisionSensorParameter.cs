using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable]
    public class VisionSensorParameter : ExposedParameter
    {
        [SerializeField] VisionSensor val;

        public override object value
        {
            get => val;
            set => val = (VisionSensor)value;
        }

        public override Type GetValueType()
        {
            return typeof(VisionSensor);
        }
    }
}