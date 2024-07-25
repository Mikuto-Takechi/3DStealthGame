using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/Decrease")]
    public class Decrease : Node
    {
        [Input, Vertical] public Node InputNode;
        [Input] public float InputValue;
        [Output] public float OutputValue;
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
            if (_isFirst)
            {
                _prevTime = Time.time;
                OutputValue = InputValue;
                _isFirst = false;
            }
            else
            {
                var currentTime = Time.time;
                OutputValue = Mathf.Clamp(InputValue - (currentTime - _prevTime), 0, float.MaxValue);
                _prevTime = currentTime;
            }

            ParameterPushed = true;
            return BTState.Success;
        }
    }
}