using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/SetFloat")]
    public class SetFloat : Node
    {
        [Input, Vertical] public Node _input;
        [SerializeField] float _value;
        [Input] public MoveController moveController;

        protected override BTState Tick()
        {
            if (moveController) moveController.ChaseTimer = _value;
            return BTState.Success;
        }
    }
}