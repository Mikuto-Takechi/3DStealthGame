using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/HasTarget")]
    public class HasTarget : Node
    {
        [Input, Vertical] public Node Input;
        [Input] public GameObject Target;

        protected override BTState Tick()
        {
            if (Target)
            {
                return BTState.Success;
            }
            else
            {
                return BTState.Failure;
            }
        }
    }
}