using System;
using GraphProcessor;

namespace MonstersDomain.BehaviorTree
{
    [NodeMenuItem("BehaviorTree/Composite/Sequence")]
    public class Sequence : Composite
    {
        [Input, Vertical] protected Node _input;

        public override BTState Tick()
        {
            var childState = _children[_activeChild].Tick();
            switch (childState)
            {
                case BTState.Success:
                    _activeChild++;
                    if (_activeChild == _children.Count)
                    {
                        _activeChild = 0;
                        return BTState.Success;
                    }

                    return BTState.Running;
                case BTState.Failure:
                    _activeChild = 0;
                    return BTState.Failure;
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