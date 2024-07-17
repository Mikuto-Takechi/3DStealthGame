using GraphProcessor;

namespace MonstersDomain.BehaviorTree
{
    /// <summary>
    ///     子ノードを並列実行します。<br />
    ///     実行した子ノードのタスクが1つでもFailureを返したタイミングで他の子ノードのタスクを全て中断して、
    ///     自身もFailureを返します。
    /// </summary>
    [NodeMenuItem("BehaviorTree/Composite/Parallel")]
    public class Parallel : Composite
    {
        [Input, Vertical] protected Node _input;

        public override BTState Tick()
        {
            var shouldWait = false;
            for (var i = 0; i < _children.Count; ++i)
            {
                var childState = _children[i].Tick();
                if (childState == BTState.Running)
                    shouldWait = true;
                else if (childState == BTState.Failure) return BTState.Failure;
            }

            if (shouldWait)
                return BTState.Running;
            return BTState.Success;
        }
    }
}