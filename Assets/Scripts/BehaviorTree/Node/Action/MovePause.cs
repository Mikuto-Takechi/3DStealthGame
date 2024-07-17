using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/MovePause")]
    public class MovePause : Node
    {
        [Input, Vertical] public Node _input;
        [SerializeField] bool _isPause;
        [Input] public MoveController moveController;

        public override BTState Tick()
        {
            inputPorts.PullDatas();
            if (moveController) moveController.Agent.isStopped = _isPause;
            return BTState.Success;
        }
    }
}