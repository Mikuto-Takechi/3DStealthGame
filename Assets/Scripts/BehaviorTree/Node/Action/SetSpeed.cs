using System;
using GraphProcessor;
using MonstersDomain.Enemy;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/SetSpeed")]
    public class SetSpeed : Node
    {
        [Input, Vertical] public Node Input;
        [SerializeField] float _speed;
        [Input] public MoveController MoveController;

        protected override BTState Tick()
        {
            if (MoveController) MoveController.Agent.speed = _speed;
            return BTState.Success;
        }
    }
}