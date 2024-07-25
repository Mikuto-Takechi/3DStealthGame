using System;
using GraphProcessor;
using MonstersDomain.Enemy;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/MoveToPoint")]
    public class MoveToPoint : Node
    {
        [Input, Vertical] public Node Input;
        [Input] public MoveController MoveController;
        [Input] public Vector3 Position;

        protected override BTState Tick()
        {
            if (!MoveController) return BTState.Failure;
            MoveController.Agent.SetDestination(Position);
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