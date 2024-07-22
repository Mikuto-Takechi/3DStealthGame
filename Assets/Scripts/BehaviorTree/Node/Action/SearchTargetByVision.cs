using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/SearchTargetByVision")]
    public class SearchTargetByVision : Node
    {
        [SerializeField] float _setTime;
        [Input, Vertical] public Node input;
        [Input] public VisionSensor sensor;
        [Output] public GameObject searchedTarget;
        [Output] public float timer;

        protected override BTState Tick()
        {
            if (!sensor) return BTState.Failure;
            Player player = null;
            if (sensor.InSightTarget(ref player))
            {
                searchedTarget = player.gameObject;
                timer = _setTime;
                ParameterPushed = true;
                return BTState.Success;
            }
            return BTState.Failure;
        }
    }
}