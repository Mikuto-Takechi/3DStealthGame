using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/CheckHeardPosition")]
    public class CheckHeardPosition : Node
    {
        [Input] public GameObject Owner;
        [Input] public HearingSensor _sensor;
        [Output] public Vector3 _position;
        [Input, Vertical] public Node _input;

        protected override BTState Tick()
        {
            PullParameters();
            if (!_sensor)
            {
                return BTState.Failure;
            }

            if (_sensor.CheckLocation != Vector3.zero)
            {
                AudioManager.Instance.Play3DSE(SE.Bite, Owner.transform.position);
                _position = _sensor.CheckLocation;
                _sensor.CheckLocation = Vector3.zero;
                PushParameters();
                return BTState.Success;
            }

            return BTState.Failure;
        }
    }
}