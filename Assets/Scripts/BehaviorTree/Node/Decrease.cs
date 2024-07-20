using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/Decrease")]
    public class Decrease : Node
    {
        [Input, Vertical] public Node input;
        [Input] public float inputValue;
        [Output] public float outputValue;
        bool _isFirst = true;
        float _prevTime;
        protected override void Enable()
        {
            _isFirst = true;
        }

        protected override void Disable()
        {
            _isFirst = true;
        }
        /// <summary>
        /// 前評価時からの経過時間を引いた値をプッシュする
        /// </summary>
        protected override BTState Tick()
        {
            PullParameters();
            if (_isFirst)
            {
                _prevTime = Time.time;
                outputValue = inputValue;
                _isFirst = false;
            }
            else
            {
                var currentTime = Time.time;
                outputValue = Mathf.Clamp(inputValue - (currentTime - _prevTime), 0, float.MaxValue);
                _prevTime = currentTime;
            }
            PushParameters();
            Debug.Log(outputValue);
            return BTState.Success;
        }
    }
}