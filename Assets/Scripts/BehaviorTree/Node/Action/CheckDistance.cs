using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/CheckDistance")]
    public class CheckDistance : Node
    {
        [SerializeField] float _distance;
        [SerializeField] bool _greaterThan;
        [Input, Vertical] public Node Input;
        [Input] public GameObject Owner;
        [Input] public GameObject Target;

        protected override BTState Tick()
        {
            if (!Owner || !Target)
            {
                return BTState.Failure;
            }
            var sqrMagnitude = (Target.transform.position - Owner.transform.position).sqrMagnitude;
            if (_greaterThan && sqrMagnitude > _distance * _distance)
            {
                Debug.Log("greater than");
                return BTState.Success;
            }

            if (!_greaterThan && sqrMagnitude <= _distance * _distance)
            {
                Debug.Log("less than");
                return BTState.Success;
            }
            return BTState.Failure;
        }
    }
}