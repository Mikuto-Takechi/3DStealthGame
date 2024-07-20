using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/HasTarget")]
    public class HasTarget : Node
    {
        [Input, Vertical] public Node _input;
        [Input] public GameObject target;

        protected override BTState Tick()
        {
            PullParameters();
            if (target)
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