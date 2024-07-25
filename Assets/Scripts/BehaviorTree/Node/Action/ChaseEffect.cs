using System;
using Cinemachine;
using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    [Serializable, NodeMenuItem("BehaviorTree/Action/ChaseEffect")]
    public class ChaseEffect : Node
    {
        [SerializeField] public bool IsStart = true;
        [Input, Vertical] public Node Input;
        [Input] public CinemachineImpulseSource ImpulseSource;

        protected override BTState Tick()
        {
            if (IsStart)
            {
                AudioManager.Instance.PlaySE(SE.Roar);
                AudioManager.Instance.PlayAmbient(Ambient.Chase);
                GameManager.Instance.ChasePostEffect(true);
                ImpulseSource.GenerateImpulse();
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