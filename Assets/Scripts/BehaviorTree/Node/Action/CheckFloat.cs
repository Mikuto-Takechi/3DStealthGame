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
        [Input] public MoveController InputValue;

        public override BTState Tick()
        {
            inputPorts.PullDatas();
            Debug.Log(InputValue.ChaseTimer);
            if (_greaterThan)
            {
                if (InputValue.ChaseTimer > _compareValue)
                {
                    return BTState.Success;
                }

                return BTState.Failure;
            }

            if (InputValue.ChaseTimer <= _compareValue)
            {
                return BTState.Success;
            }
                
            return BTState.Failure;
        }
    }
}