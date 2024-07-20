using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/ClearTarget")]
    public class ClearTarget : Node
    {
        [Input, Vertical] public Node _input;
        [Output] public GameObject target;

        protected override BTState Tick()
        {
            target = null;
            PushParameters();
            return BTState.Success;
        }
    }
}