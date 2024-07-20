using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using MonstersDomain.BehaviorTree;

namespace MonstersDomain
{
    public class BehaviorTreeProcessor : BaseGraphProcessor
    {
        List<BaseNode> _processList;

        public BehaviorTreeProcessor(BaseGraph graph) : base(graph)
        {
        }

        public override void UpdateComputeOrder()
        {
            //graph.UpdateComputeOrder(ComputeOrderType.BreadthFirst);
            _processList = graph.nodes.OrderBy(n => n.computeOrder).ToList();
        }

        public override void Run()
        {
            // var count = _processList.Count;
            // // すべてのノードを順番に処理する
            // for (var i = 0; i < count; i++)
            // { 
            //     _processList[i].OnProcess();
            // }
            // JobHandle.ScheduleBatchedJobs();
        }

        public void UpdateTick()
        {
            var root = _processList.OfType<Root>().First();
            root.OnTick();
        }
    }
}