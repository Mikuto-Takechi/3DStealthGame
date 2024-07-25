using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/SearchTargetByVision")]
    public class SearchTargetByVision : Node
    {
        [SerializeField] float _setTime;
        [Input, Vertical] public Node Input;
        [Input] public VisionSensor Sensor;
        [Output] public GameObject SearchedTarget;
        [Output] public float Timer;

        protected override BTState Tick()
        {
            if (!Sensor) return BTState.Failure;
            Player player = null;
            if (Sensor.InSightTarget(ref player))
            {
                SearchedTarget = player.gameObject;
                Timer = _setTime;
                ParameterPushed = true;
                return BTState.Success;
            }
            return BTState.Failure;
        }
    }
}