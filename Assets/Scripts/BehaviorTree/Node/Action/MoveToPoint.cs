using System;
using GraphProcessor;
using MonstersDomain.Enemy;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/MoveToPoint")]
    public class MoveToPoint : Node
    {
        [Input, Vertical] public Node _input;
        [Input] public MoveController _moveController;
        [Input] public Vector3 _position;

        protected override BTState Tick()
        {
            if (!_moveController) return BTState.Failure;
            _moveController.Agent.SetDestination(_position);
            return BTState.Success;
            // switch (_moveController.PointAsyncMover.Current)
            // {
            //     case MoveState.Complete:
            //         _moveController.PointAsyncMover.Current = MoveState.Ready;
            //         return BTState.Success;
            //     case MoveState.Running:
            //         return BTState.Running;
            //     case MoveState.Ready:
            //         _moveController.MoveToPoint(_position);
            //         return BTState.Running;
            //     case MoveState.Abort:
            //         _moveController.MoveToPoint(_position);
            //         return BTState.Running;
            //     // _moveController.PointAsyncMover.Current = MoveState.Ready;
            //     // return BTState.Failure;
            //     default:
            //         return BTState.Failure;
            // }
        }
    }
}