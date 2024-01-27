using UnityEngine;
using UnityEngine.UI;

namespace MonstersDomain
{
    /// <summary>
    ///     中心に向かって縮むUI
    /// </summary>
    [RequireComponent(typeof(RectMask2D))]
    public class ShrinkBar : MonoBehaviour
    {
        RectMask2D _rectMask;
        RectTransform _rect;

        void Awake()
        {
            _rectMask = GetComponent<RectMask2D>();
            _rect = GetComponent<RectTransform>();
        }

        public void SetFill(float baseValue, float targetValue)
        {
            var ratio = targetValue / baseValue;
            var answer = (_rect.rect.width - _rect.rect.width * ratio) / 2;
            var padding = _rectMask.padding;
            _rectMask.padding = new Vector4(answer, padding.y, answer, padding.w);
        }
    }
}