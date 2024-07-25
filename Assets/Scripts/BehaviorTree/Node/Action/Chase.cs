using System;
using GraphProcessor;
using MonstersDomain.Enemy;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/Chase")]
    public class Chase : Node
    {
        [Input, Vertical] public Node Input;
        [Input] public MoveController MoveController;
        [Input] public GameObject Target;

        protected override BTState Tick()
        {
            if (MoveController && Target)
            {
                MoveController.Chase(Target.transform);
                return BTState.Success;
            }

            return BTState.Failure;
        }
    }
}