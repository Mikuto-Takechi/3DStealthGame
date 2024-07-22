using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/Log")]
    public class Log : Node
    {
        [Input, Vertical] public Node parent;
        [Input("In")] public bool input;
        protected override BTState Tick()
        {
            if (input)
                Debug.Log($"Target is {input}");
            else
                Debug.Log("Target is null");
            return BTState.Success;
        }
    }
}