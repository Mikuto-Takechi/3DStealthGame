using System;
using DG.Tweening;
using UnityEngine;

namespace MonstersDomain
{
    public class SettingsUI : MonoBehaviour
    {
        CanvasGroup _canvasGroup;
        float _initialXAnchoredPosition;
        RectTransform _rect;
        Sequence _sequence;
        public bool IsOpen { get; private set; }

        void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _initialXAnchoredPosition = _rect.anchoredPosition.x;
        }

        public void Switch()
        {
            if (!IsOpen) Open();
            else Close();
        }

        public void Open()
        {
            if (_sequence != null) return;
            IsOpen = true;
            var temp = _rect.anchoredPosition;
            temp.x -= _rect.rect.width;
            _rect.anchoredPosition = temp;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            _sequence = DOTween.Sequence().OnComplete(() => _sequence = null).SetLink(gameObject);
            _sequence.Join(_rect.DOAnchorPosX(_initialXAnchoredPosition, 0.5f));
            _sequence.Join(DOVirtual.Float(0, 1, 0.5f, value => _canvasGroup.alpha = value));
        }

        public void Close(Action callback = null)
        {
            if (_sequence != null) return;
            IsOpen = false;
            var temp = _rect.anchoredPosition;
            temp.x = _initialXAnchoredPosition;
            _rect.anchoredPosition = temp;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _sequence = DOTween.Sequence().OnComplete(() =>
            {
                _sequence = null;
                callback?.Invoke();
            }).SetLink(gameObject);
            _sequence.Join(_rect.DOAnchorPosX(_initialXAnchoredPosition - _rect.rect.width, 0.5f));
            _sequence.Join(DOVirtual.Float(1, 0, 0.5f, value => _canvasGroup.alpha = value));
        }
    }
}