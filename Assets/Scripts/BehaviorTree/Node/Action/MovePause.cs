using System;
using GraphProcessor;
using MonstersDomain.Enemy;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/MovePause")]
    public class MovePause : Node
    {
        [Input, Vertical] public Node Input;
        [SerializeField] bool _isPause;
        [Input] public MoveController MoveController;

        protected override BTState Tick()
        {
            if (MoveController) MoveController.Agent.isStopped = _isPause;
            return BTState.Success;
        }
    }
}