using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/MoveToPoint")]
    public class MoveToPoint : Node
    {
        [Input, Vertical] public Node _input;
        [Input] public MoveController _moveController;
        [Input] public Vector3 _position;

        public override BTState Tick()
        {
            inputPorts.PullDatas();
            Debug.Log(nameof(MoveToPoint));
            if (!_moveController) return BTState.Failure;

            switch (_moveController.PointAsyncMover.Current)
            {
                case MoveState.Complete:
                    _moveController.PointAsyncMover.Current = MoveState.Ready;
                    return BTState.Success;
                case MoveState.Running:
                    return BTState.Running;
                case MoveState.Ready:
                    _moveController.MoveToPoint(_position);
                    return BTState.Running;
                case MoveState.Abort:
                    _moveController.MoveToPoint(_position);
                    return BTState.Running;
                // _moveController.PointAsyncMover.Current = MoveState.Ready;
                // return BTState.Failure;
                default:
                    return BTState.Failure;
            }
        }
    }
}