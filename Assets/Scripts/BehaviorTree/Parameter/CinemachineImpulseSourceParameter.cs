using System;
using Cinemachine;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable]
    public class CinemachineImpulseSourceParameter : ExposedParameter
    {
        [SerializeField] CinemachineImpulseSource val;

        public override object value
        {
            get => val;
            set => val = (CinemachineImpulseSource)value;
        }

        public override Type GetValueType()
        {
            return typeof(CinemachineImpulseSource);
        }
    }
}