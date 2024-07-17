using System;
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
        [SerializeField] readonly bool _shuffle = false;

        protected override void Process()
        {
            base.Process();
            if (_shuffle)
            {
                var n = _children.Count;
                while (n > 1)
                {
                    n--;
                    var k = Mathf.FloorToInt(Random.value * (n + 1));
                    var value = _children[k];
                    _children[k] = _children[n];
                    _children[n] = value;
                }
            }
        }

        public override BTState Tick()
        {
            var childState = _children[_activeChild].Tick();
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