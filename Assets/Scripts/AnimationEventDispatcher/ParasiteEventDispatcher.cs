using UniRx;
using UnityEngine;

namespace MonstersDomain
{
    public class ParasiteEventDispatcher : MonoBehaviour
    {
        public readonly Subject<Unit> EventFootSteps = new();

        void FootSteps()
        {
            EventFootSteps.OnNext(Unit.Default);
        }
    }
}