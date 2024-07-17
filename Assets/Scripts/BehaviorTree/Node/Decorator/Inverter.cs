using GraphProcessor;

namespace MonstersDomain.BehaviorTree
{
    /// <summary>
    ///     子ノードがFailureを返した時にはSuccessを返し、Successを返したときにはFailureを返す。<br />
    ///     子ノードがRunningを返した時には、このノードもRunningを返す。
    /// </summary>
    [NodeMenuItem("BehaviorTree/Decorator/Inverter")]
    public class Inverter : Decorator
    {
        public override BTState Tick()
        {
            switch (Child.Tick())
            {
                case BTState.Running:
                    return BTState.Running;
                case BTState.Failure:
                    return BTState.Success;
                case BTState.Success:
                    return BTState.Failure;
                default:
                    return BTState.Running;
            }
        }
    }
}