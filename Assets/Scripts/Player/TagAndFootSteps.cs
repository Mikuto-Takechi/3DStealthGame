using System;
using UnityEngine;

namespace MonstersDomain
{
    [Serializable]
    internal struct TagAndFootSteps
    {
        [SerializeField] string _tag;
        [SerializeField] FootSteps _footSteps;
        public string Tag => _tag;
        public FootSteps FootSteps => _footSteps;
    }
}