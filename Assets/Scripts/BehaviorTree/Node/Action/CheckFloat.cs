using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/CheckFloat")]
    public class CheckFloat : Node
    {
        [SerializeField] float _compareValue;
        [SerializeField] bool _greaterThan;
        [Input, Vertical] public Node Input;
        [Input] public float InputValue;

        protected override BTState Tick()
        {
            if (_greaterThan)
            {
                if (InputValue > _compareValue)
                {
                    return BTState.Success;
                }

                return BTState.Failure;
            }

            if (InputValue <= _compareValue)
            {
                return BTState.Success;
            }
                
            return BTState.Failure;
        }
    }
}