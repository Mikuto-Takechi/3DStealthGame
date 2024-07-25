using System;
using System.Linq;
using GraphProcessor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MonstersDomain.BehaviorTree
{
    /// <summary>
    ///     子が成功するまで各子を実行し、成功を返す。<br />
    ///     成功した子プロセスがない場合は失敗を返す。
    /// </summary>
    [NodeMenuItem("BehaviorTree/Composite/Selector")]
    public class Selector : Composite
    {
        [Input, Vertical] protected Node Input;

        protected override BTState Tick()
        {
            var childState = Children[ActiveChild].OnTick();
            switch (childState)
            {
                case BTState.Success:
                    ActiveChild = 0;
                    return BTState.Success;
                case BTState.Failure:
                    ActiveChild++;
                    if (ActiveChild == Children.Count)
                    {
                        ActiveChild = 0;
                        return BTState.Failure;
                    }

                    return BTState.Running;
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