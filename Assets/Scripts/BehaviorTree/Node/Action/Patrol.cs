using System;
using GraphProcessor;
using MonstersDomain.Enemy;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/Patrol")]
    public class Patrol : Node
    {
        [Input, Vertical] public Node Input;
        [Input] public MoveController MoveController;

        protected override BTState Tick()
        {
            if (MoveController)
            {
                MoveController.Patrol();
                return BTState.Success;
            }

            return BTState.Failure;
        }
    }
}