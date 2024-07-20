using System;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/SetTrigger")]
    public class SetTrigger : Node
    {
        [Input, Vertical] public Node _input;
        [SerializeField] string _triggerName;
        [Input] public Animator animator;

        protected override BTState Tick()
        {
            PullParameters();
            animator.SetTrigger(_triggerName);
            return BTState.Success;
        }
    }
}