using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("Custom/Logger")]
    public class Logger : BaseNode
    {
        [Input("In")] public bool input;

        protected override void Process()
        {
            Debug.Log(input);
        }
    }

    [Serializable, NodeMenuItem("BehaviorTree/Action/Log")]
    public class Log : Node
    {
        [Input("In")] public GameObject input;

        protected override BTState Tick()
        {
            PullParameters();
            if (input)
                Debug.Log($"Target is {input.name}");
            else
                Debug.Log("Target is null");
            return BTState.Success;
        }
    }
}