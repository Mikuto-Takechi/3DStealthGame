using System;
using UnityEngine;

namespace MonstersDomain
{
    /// <summary>
    /// タグと足音のenumを関連付けるための構造体
    /// </summary>
    [Serializable]
    struct TagAndFootSteps
    {
        [SerializeField] string _tag;
        [SerializeField] FootSteps _footSteps;
        public string Tag => _tag;
        public FootSteps FootSteps => _footSteps;
    }
}