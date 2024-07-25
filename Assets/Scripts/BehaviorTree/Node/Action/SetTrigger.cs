using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/SetTrigger")]
    public class SetTrigger : Node
    {
        [Input, Vertical] public Node Input;
        [SerializeField] string _triggerName;
        [Input] public Animator Animator;

        protected override BTState Tick()
        {
            Animator.SetTrigger(_triggerName);
            return BTState.Success;
        }
    }
}