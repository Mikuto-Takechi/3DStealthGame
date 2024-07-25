using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/CheckHeardPosition")]
    public class CheckHeardPosition : Node
    {
        [Input] public GameObject Owner;
        [Input] public HearingSensor Sensor;
        [Output] public Vector3 Position;
        [Input, Vertical] public Node Input;

        protected override BTState Tick()
        {
            if (!Sensor)
            {
                return BTState.Failure;
            }

            if (Sensor.CheckLocation != Vector3.zero)
            {
                AudioManager.Instance.Play3DSE(SE.Bite, Owner.transform.position);
                Position = Sensor.CheckLocation;
                Sensor.CheckLocation = Vector3.zero;
                ParameterPushed = true;
                return BTState.Success;
            }

            return BTState.Failure;
        }
    }
}