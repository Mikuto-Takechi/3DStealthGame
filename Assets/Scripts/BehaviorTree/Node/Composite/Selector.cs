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
        [Input, Vertical] protected Node _input;

        protected override BTState Tick()
        {
            var childState = _children[_activeChild].OnTick();
            switch (childState)
            {
                case BTState.Success:
                    _activeChild = 0;
                    return BTState.Success;
                case BTState.Failure:
                    _activeChild++;
                    if (_activeChild == _children.Count)
                    {
                        _activeChild = 0;
                        return BTState.Failure;
                    }

                    return BTState.Running;
                case BTState.Running:
                    return BTState.Running;
                case BTState.Abort:
                    _activeChild = 0;
                    return BTState.Abort;
            }

            throw new Exception("Tick関数のswitch文を抜けてしまった。");
        }
    }
}