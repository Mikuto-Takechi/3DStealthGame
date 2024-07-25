using System.Collections;
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
        [SerializeField] float _fadeOutTimer = 1f;
        [SerializeField] CanvasGroup _canvasGroup;
        Coroutine _fadeIn, _fadeOut;
        float _fadeTimer;
        bool _recentlyChanged = true;
        RectTransform _rect;
        RectMask2D _rectMask;

        void Awake()
        {
            _rectMask = GetComponent<RectMask2D>();
            _rect = GetComponent<RectTransform>();
        }

        void Update()
        {
            if (!_canvasGroup) return;

            if (_fadeTimer > 0)
            {
                _fadeTimer -= Time.deltaTime;
            }
            else if (_recentlyChanged)
            {
                _recentlyChanged = false;
                if (_fadeIn != null) StopCoroutine(_fadeIn);
                _fadeOut = StartCoroutine(FadeOut());
            }
        }

        public void SetFill(float baseValue, float targetValue)
        {
            var ratio = targetValue / baseValue;
            var answer = (_rect.rect.width - _rect.rect.width * ratio) / 2;
            var padding = _rectMask.padding;
            _rectMask.padding = new Vector4(answer, padding.y, answer, padding.w);
            if (_canvasGroup)
            {
                if (!_recentlyChanged)
                {
                    if (_fadeOut != null) StopCoroutine(_fadeOut);
                    _fadeIn = StartCoroutine(FadeIn());
                    _recentlyChanged = true;
                }

                _fadeTimer = _fadeOutTimer;
            }
        }

        IEnumerator FadeOut()
        {
            while (true)
            {
                _canvasGroup.alpha -= Time.deltaTime;
                if (_canvasGroup.alpha <= 0) break;
                yield return null;
            }
        }

        IEnumerator FadeIn()
        {
            while (true)
            {
                _canvasGroup.alpha += Time.deltaTime;
                if (_canvasGroup.alpha >= 1) break;
                yield return null;
            }
        }
    }
}