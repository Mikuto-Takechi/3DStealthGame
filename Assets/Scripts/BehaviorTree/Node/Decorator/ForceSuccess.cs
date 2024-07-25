using GraphProcessor;

namespace MonstersDomain.BehaviorTree
{
    /// <summary>
    ///     子ノードがRunningを返した時には、このノードもRunningを返す。<br />
    ///     そうでない場合は、常にSuccessを返す。
    /// </summary>
    [NodeMenuItem("BehaviorTree/Decorator/ForceSuccess")]
    public class ForceSuccess : Decorator
    {
        protected override BTState Tick()
        {
            switch (Child.OnTick())
            {
                case BTState.Running:
                    return BTState.Running;
                default:
                    return BTState.Success;
            }
        }
    }
}