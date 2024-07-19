using System;
using System.Linq;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/Decrease")]
    public class Decrease : Node
    {
        [Input, Vertical] public Node input;
        [Input] public float inputValue;
        [Output] public float outputValue;
        public override BTState Tick()
        {
            inputPorts.PullDatas();
            outputValue = inputValue - 1f;
            Debug.Log(outputValue);
            outputPorts.PushDatas();
            return BTState.Success;
        }
    }
}