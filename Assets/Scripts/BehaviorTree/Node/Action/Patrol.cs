using System;
using GraphProcessor;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/Patrol")]
    public class Patrol : Node
    {
        [Input, Vertical] public Node _input;
        [Input] public MoveController _moveController;

        public override BTState Tick()
        {
            inputPorts.PullDatas();
            if (_moveController)
            {
                _moveController.Patrol();
                return BTState.Success;
            }

            return BTState.Failure;
        }
    }
}