using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    /// <summary>
    ///     指定した秒数Running状態を維持してから子ノードの評価を始めるデコレータ。
    /// </summary>
    [NodeMenuItem("BehaviorTree/Decorator/Delay")]
    public class Delay : Decorator
    {
        float _future = -1;
        [SerializeField] float _seconds = 0;

        public override BTState Tick()
        {
            if (_future < 0)
                _future = Time.time + _seconds;

            if (Time.time >= _future)
                switch (Child.Tick())
                {
                    case BTState.Running:
                        return BTState.Running;
                    case BTState.Failure:
                        _future = -1;
                        return BTState.Failure;
                    case BTState.Success:
                        _future = -1;
                        return BTState.Success;
                    default:
                        return BTState.Running;
                }

            return BTState.Running;
        }
    }
}