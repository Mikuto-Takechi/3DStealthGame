using GraphProcessor;

namespace MonstersDomain.BehaviorTree
{
    [NodeMenuItem("BehaviorTree/Decorator/If")]
    public class If : Decorator
    {
        [Input] public bool Predicate;

        protected override BTState Tick()
        {
            PullParameters();
            if (Predicate)
            {
                return BTState.Success;
            }
            return Child.OnTick();
        }
    }
}