using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("Custom/Decrease")]
    public class Decrease : BaseNode
    {
        [Output] public float output;
        protected override void Process()
        {
            output = Mathf.Clamp(output - Time.deltaTime, 0, float.MaxValue);
            
            Debug.Log(output);
        }
    }
}