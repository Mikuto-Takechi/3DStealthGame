using System.Collections.Generic;
using System.Linq;
using GraphProcessor;

namespace MonstersDomain.BehaviorTree
{
    public abstract class Composite : Node
    {
        protected int _activeChild;
        protected List<Node> _children = new();

        [Output(allowMultiple: true), Vertical]
        protected Node _output;

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
            _children = new List<Node>();
            var outputNodes = GetOutputNodes().OrderBy(x => x.position.x);
            foreach (var outputNode in outputNodes)
            {
                if (outputNode is Node node)
                {
                    _children.Add(node);
                }
            }
        }
    }
}