using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    /// <summary>
    ///     指定秒数待ってからSuccessを返す。
    /// </summary>
    [Serializable, NodeMenuItem("BehaviorTree/Action/Wait")]
    public class Wait : Node
    {
        [SerializeField] float _seconds = 0;
        float _future = -1;
        [Input, Vertical] public Node input;

        protected override BTState Tick()
        {
            if (_future < 0)
                _future = Time.time + _seconds;

            if (Time.time >= _future)
            {
                _future = -1;
                return BTState.Success;
            }

            return BTState.Running;
        }
    }
}