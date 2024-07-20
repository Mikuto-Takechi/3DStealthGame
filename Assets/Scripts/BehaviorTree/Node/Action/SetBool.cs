using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/SetBool")]
    public class SetBool : Node
    {
        [Input, Vertical] public Node _input;
        [Output] public bool _output;
        [SerializeField] bool _setBool;

        protected override BTState Tick()
        {
            _output = _setBool;
            PushParameters();
            return BTState.Success;
        }
    }
}