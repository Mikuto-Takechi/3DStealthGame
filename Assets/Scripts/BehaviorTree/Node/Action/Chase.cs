using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/Chase")]
    public class Chase : Node
    {
        [Input, Vertical] public Node _input;
        [Input] public MoveController _moveController;
        [Input] public GameObject _target;

        protected override BTState Tick()
        {
            if (_moveController && _target)
            {
                _moveController.Chase(_target.transform);
                return BTState.Success;
            }

            return BTState.Failure;
        }
    }
}