using System;

namespace MonstersDomain.BehaviorTree
{
    [Serializable]
    public class Root : Composite
    {
        //  終了フラグ。立っていたらこれ以上処理しない。
        bool _isTerminated;

        public override BTState Tick()
        {
            if (_isTerminated) return BTState.Abort;
            while (true)
                switch (_children[_activeChild].Tick())
                {
                    case BTState.Running:
                        return BTState.Running;
                    case BTState.Abort:
                        _isTerminated = true;
                        return BTState.Abort;
                    default:
                        _activeChild++;
                        if (_activeChild == _children.Count)
                        {
                            _activeChild = 0;
                            return BTState.Success;
                        }

                        continue;
                }
        }
    }
}