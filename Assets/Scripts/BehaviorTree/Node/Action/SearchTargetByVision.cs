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
        [Input] public MoveController moveController;

        public override BTState Tick()
        {
            inputPorts.PullDatas();
            if (!sensor) return BTState.Failure;

            Player player = null;
            if (sensor.InSightTarget(ref player))
            {
                searchedTarget = player.gameObject;
                moveController.ChaseTimer = _setTime;
                outputPorts.PushDatas();
                return BTState.Success;
            }
            return BTState.Failure;
        }
    }
}