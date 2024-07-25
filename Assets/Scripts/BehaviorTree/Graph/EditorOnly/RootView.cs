#if UNITY_EDITOR
using GraphProcessor;
using MonstersDomain.BehaviorTree;

namespace MonstersDomain
{
    [NodeCustomEditor(typeof(Root))]
    public class RootView : BaseNodeView
    {
    }
}
#endif