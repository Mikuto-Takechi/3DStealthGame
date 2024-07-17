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
        [Input, Vertical] public Node _input;
        [Input] public GameObject _owner;
        [Input] public GameObject _target;

        public override BTState Tick()
        {
            inputPorts.PullDatas();
            if (!_owner || !_target)
            {
                return BTState.Failure;
            }
            var sqrMagnitude = (_target.transform.position - _owner.transform.position).sqrMagnitude;
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