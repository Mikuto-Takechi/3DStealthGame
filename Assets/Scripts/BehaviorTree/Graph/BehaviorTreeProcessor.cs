using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using GraphProcessor;
using MonstersDomain.BehaviorTree;

namespace MonstersDomain
{
    /// <summary>
    /// グラフを実行するクラス。(従来の処理とは違い実行順序を決める必要が無いのでこのクラスを挟んでOnTickを呼ぶ必要は無い)
    /// </summary>
    public class BehaviorTreeProcessor : BaseGraphProcessor
    {
        Root _root;

        public BehaviorTreeProcessor(BaseGraph graph) : base(graph)
        {
            _root = graph.nodes.OfType<Root>().First();
        }

        public override void UpdateComputeOrder() { }
        public override void Run() { }
        public void UpdateTick()
        {
            _root.OnTick();
        }

        public async UniTaskVoid UnHighlight(CancellationToken ct)
        {
            while (true)
            {
                foreach (var node in graph.nodes.OfType<Node>())
                {
                    node.HighLighted = false;
                }

                await UniTask.WaitForSeconds(0.8f, cancellationToken: ct);
            }
        }
    }
}