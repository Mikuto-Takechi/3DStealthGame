using System;
using System.Linq;
using GraphProcessor;

namespace MonstersDomain.BehaviorTree
{
    [Serializable]
    public abstract class Node : BaseNode
    {
        protected abstract BTState Tick();
        public BTState OnTick()
        {
            inputPorts.PullDatas();
            var state = Tick();
            outputPorts.PushDatas();
            return state;
        }
        protected void PullParameters()
        {
            foreach (var node in GetInputNodes().OfType<ParameterNode>())
            {
                node.OnProcess();
            }
            //inputPorts.PullDatas();
        }

        protected void PushParameters()
        {
            //outputPorts.PushDatas();
            foreach (var node in GetOutputNodes().OfType<ParameterNode>())
            {
                node.OnProcess();
            }
        }
    }
}