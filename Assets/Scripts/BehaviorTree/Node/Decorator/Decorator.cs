using System.Linq;
using GraphProcessor;

namespace MonstersDomain.BehaviorTree
{
    public abstract class Decorator : Node
    {
        public Node Child;
        [Input, Vertical] public Node Input;
        [Output, Vertical] public Node Output;

        protected override void Process()
        {
            Child = GetOutputNodes().ToList().OfType<Node>().First();
        }
    }
}