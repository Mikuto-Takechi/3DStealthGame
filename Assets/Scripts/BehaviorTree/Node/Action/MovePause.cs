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

        protected override BTState Tick()
        {
            if (moveController) moveController.Agent.isStopped = _isPause;
            return BTState.Success;
        }
    }
}