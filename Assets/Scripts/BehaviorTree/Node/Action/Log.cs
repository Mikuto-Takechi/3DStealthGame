using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/Log")]
    public class Log : Node
    {
        [Input, Vertical] public Node Parent;
        [Input("In")] public bool Input;
        protected override BTState Tick()
        {
            if (Input)
                Debug.Log($"Target is {Input}");
            else
                Debug.Log("Target is null");
            return BTState.Success;
        }
    }
}