using System.Collections.Generic;
using System.Linq;
using GraphProcessor;

namespace MonstersDomain.BehaviorTree
{
    public abstract class Composite : Node
    {
        protected int ActiveChild;
        protected List<Node> Children = new();

        [Output(allowMultiple: true), Vertical] protected Node Output;

        protected override void Enable()
        {
            onAfterEdgeConnected += UpdateChildren;
            onAfterEdgeDisconnected += UpdateChildren;
            UpdateChildren(null);
        }

        protected override void Disable()
        {
            onAfterEdgeConnected -= UpdateChildren;
            onAfterEdgeDisconnected -= UpdateChildren;
        }

        void UpdateChildren(SerializableEdge _)
        {
            Children = new List<Node>();
            var outputNodes = GetOutputNodes().OrderBy(x => x.position.x);
            foreach (var outputNode in outputNodes)
            {
                if (outputNode is Node node)
                {
                    Children.Add(node);
                }
            }
        }
    }
}