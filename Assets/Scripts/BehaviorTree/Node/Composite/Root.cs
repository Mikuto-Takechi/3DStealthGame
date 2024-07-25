using System;
using System.Linq;

namespace MonstersDomain.BehaviorTree
{
    [Serializable]
    public class Root : Composite
    {
        //  終了フラグ。立っていたらこれ以上処理しない。
        bool _isTerminated;

        protected override BTState Tick()
        {
            if (_isTerminated) return BTState.Abort;
            while (true)
                switch (Children[ActiveChild].OnTick())
                {
                    case BTState.Running:
                        return BTState.Running;
                    case BTState.Abort:
                        _isTerminated = true;
                        return BTState.Abort;
                    default:
                        ActiveChild++;
                        if (ActiveChild == Children.Count)
                        {
                            ActiveChild = 0;
                            return BTState.Success;
                        }

                        continue;
                }
        }
    }
}