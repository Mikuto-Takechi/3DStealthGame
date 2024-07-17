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

        protected override void Process()
        {
            _children = new List<Node>();
            var outputNodes = GetOutputNodes().OrderBy(x => x.position.x);
            foreach (var outputNode in outputNodes)
                if (outputNode is Node)
                    _children.Add(outputNode as Node);
        }

        public virtual void ResetChildren()
        {
            _activeChild = 0;
            for (var i = 0; i < _children.Count; i++)
            {
                var b = _children[i] as Composite;
                if (b != null) b.ResetChildren();
            }
        }
    }
}