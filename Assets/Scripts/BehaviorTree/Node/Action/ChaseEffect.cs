using System;
using Cinemachine;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/ChaseEffect")]
    public class ChaseEffect : Node
    {
        [SerializeField] public bool _isStart = true;
        [Input, Vertical] public Node _input;
        [Input] public CinemachineImpulseSource impulseSource;

        protected override BTState Tick()
        {
            PullParameters();
            if (_isStart)
            {
                AudioManager.Instance.PlaySE(SE.Roar);
                AudioManager.Instance.PlayAmbient(Ambient.Chase);
                GameManager.Instance.ChasePostEffect(true);
                impulseSource.GenerateImpulse();
            }
            else
            {
                AudioManager.Instance.StopMusic();
                AudioManager.Instance.StopAmbient();
                GameManager.Instance.ChasePostEffect(false);
            }

            return BTState.Success;
        }
    }
}