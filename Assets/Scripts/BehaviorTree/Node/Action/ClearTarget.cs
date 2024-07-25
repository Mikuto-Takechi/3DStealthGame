using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/ClearTarget")]
    public class ClearTarget : Node
    {
        [Input, Vertical] public Node Input;
        [Output] public GameObject Target;

        protected override BTState Tick()
        {
            Target = null;
            ParameterPushed = true;
            return BTState.Success;
        }
    }
}