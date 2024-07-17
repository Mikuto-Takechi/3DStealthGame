using System;
using GraphProcessor;

namespace MonstersDomain.BehaviorTree
{
    [Serializable]
    public abstract class Node : BaseNode
    {
        public abstract BTState Tick();
    }
}