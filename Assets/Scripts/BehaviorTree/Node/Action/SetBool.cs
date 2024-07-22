using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/SetBool")]
    public class SetBool : Node
    {
        [Input, Vertical] public Node _input;
        [Output] public bool OutputValue;
        [SerializeField] bool _setBool;

        protected override BTState Tick()
        {
            OutputValue = _setBool;
            ParameterPushed = true;
            return BTState.Success;
        }
    }
}