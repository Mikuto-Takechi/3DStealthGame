using System;
using System.Linq;
using GraphProcessor;

namespace MonstersDomain.BehaviorTree
{
    [Serializable]
    public abstract class Node : BaseNode
    {
        public bool HighLighted { get; set; }
        public Action OnTicked { get; set; }
        protected abstract BTState Tick();
        protected bool ParameterPushed;
        public BTState OnTick()
        {
            //  ノードに繋がっているパラメーターノードの値をプッシュする。
            PullParameters();
            //  実行されたのでエディタ上でハイライトにする。
            HighLighted = true;
            //  プッシュされたパラメーターノードの値を自身にプルする。
            inputPorts.PullDatas();
            //  ノードの処理を実行する。
            var state = Tick();
            //  Output属性の付いた変数の値をプッシュする。
            outputPorts.PushDatas();
            OnTicked?.Invoke();
            //  Output属性の付いた変数が更新されていた場合(boolの値は直接変える必要がある)
            if (ParameterPushed)
            {
                //  Outputに接続されているパラメーターノードで値をプルする。
                PushParameters();
                ParameterPushed = false;
            }
            return state;
        }
        void PullParameters()
        {
            var inputNodes = GetInputNodes();
            if (inputNodes.Any())
            {
                foreach (var node in inputNodes.OfType<ParameterNode>())
                {
                    node.OnProcess();
                }
            }
        }
        
        void PushParameters()
        {
            var outputNodes = GetOutputNodes();
            if (outputNodes.Any())
            {
                foreach (var node in outputNodes.OfType<ParameterNode>())
                {
                    node.OnProcess();
                }
            }
        }
    }
}