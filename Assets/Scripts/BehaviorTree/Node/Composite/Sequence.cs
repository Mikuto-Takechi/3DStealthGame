using System;
using GraphProcessor;

namespace MonstersDomain.BehaviorTree
{
    [NodeMenuItem("BehaviorTree/Composite/Sequence")]
    public class Sequence : Composite
    {
        [Input, Vertical] protected Node Input;

        protected override BTState Tick()
        {
            var childState = Children[ActiveChild].OnTick();
            switch (childState)
            {
                case BTState.Success:
                    ActiveChild++;
                    if (ActiveChild == Children.Count)
                    {
                        ActiveChild = 0;
                        return BTState.Success;
                    }

                    return BTState.Running;
                case BTState.Failure:
                    ActiveChild = 0;
                    return BTState.Failure;
                case BTState.Running:
                    return BTState.Running;
                case BTState.Abort:
                    ActiveChild = 0;
                    return BTState.Abort;
            }

            throw new Exception("Tick関数のswitch文を抜けてしまった。");
        }
    }
}