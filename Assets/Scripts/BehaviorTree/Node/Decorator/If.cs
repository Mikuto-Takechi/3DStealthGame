using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [NodeMenuItem("BehaviorTree/Decorator/If")]
    public class If : Decorator
    {
        [Input] public bool Predicate;

        public override BTState Tick()
        {
            inputPorts.PullDatas();
            if (Predicate)
            {
                return BTState.Success;
            }
            return Child.Tick();
        }
    }
}