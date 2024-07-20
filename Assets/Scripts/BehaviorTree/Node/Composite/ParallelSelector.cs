using GraphProcessor;

namespace MonstersDomain.BehaviorTree
{
    /// <summary>
    ///     子ノードのタスクを並列実行します。<br />
    ///     実行した子ノードのタスクが1つでもSuccessを返したタイミングで他の子ノードのタスクを全て中断して、
    ///     自身もSuccessを返します。
    /// </summary>
    [NodeMenuItem("BehaviorTree/Composite/ParallelSelector")]
    public class ParallelSelector : Composite
    {
        [Input, Vertical] protected Node _input;

        protected override BTState Tick()
        {
            var shouldWait = false;
            for (var i = 0; i < _children.Count; ++i)
            {
                var childState = _children[i].OnTick();
                if (childState == BTState.Running)
                    shouldWait = true;
                else if (childState == BTState.Success) return BTState.Success;
            }

            if (shouldWait)
                return BTState.Running;
            return BTState.Failure;
        }
    }
}