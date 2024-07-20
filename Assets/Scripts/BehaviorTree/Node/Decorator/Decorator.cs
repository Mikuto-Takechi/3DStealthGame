using System.Linq;
using GraphProcessor;

namespace MonstersDomain.BehaviorTree
{
    public abstract class Decorator : Node
    {
        public Node Child;
        [Input, Vertical] public Node Input;
        [Output, Vertical] public Node Output;
        protected override void Enable()
        {
            onAfterEdgeConnected += UpdateChild;
            onAfterEdgeDisconnected += UpdateChild;
            UpdateChild(null);
        }

        protected override void Disable()
        {
            onAfterEdgeConnected -= UpdateChild;
            onAfterEdgeDisconnected -= UpdateChild;
        }

        void UpdateChild(SerializableEdge _)
        {
            Child = GetOutputNodes().ToList().OfType<Node>().FirstOrDefault();
        }
    }
}