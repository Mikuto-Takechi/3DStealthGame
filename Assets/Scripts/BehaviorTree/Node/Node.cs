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
            PullParameters();
            HighLighted = true;
            inputPorts.PullDatas();
            var state = Tick();
            outputPorts.PushDatas();
            OnTicked?.Invoke();
            if (ParameterPushed)
            {
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