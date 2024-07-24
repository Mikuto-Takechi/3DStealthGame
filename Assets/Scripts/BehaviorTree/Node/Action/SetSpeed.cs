using System;
using GraphProcessor;
using MonstersDomain.Enemy;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/SetSpeed")]
    public class SetSpeed : Node
    {
        [Input, Vertical] public Node _input;
        [SerializeField] float _speed;
        [Input] public MoveController moveController;

        protected override BTState Tick()
        {
            if (moveController) moveController.Agent.speed = _speed;
            return BTState.Success;
        }
    }
}