using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MonstersDomain
{
    public class SelectEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        Tween _tween;

        public void OnPointerClick(PointerEventData eventData)
        {
            AudioManager.Instance.PlaySE(SE.ButtonSubmit);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _tween?.Kill();
            AudioManager.Instance.PlaySE(SE.ButtonSelect);
            _tween = transform.DOScale(1.2f, 0.3f).SetLink(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _tween?.Kill();
            _tween = transform.DOScale(1, 0.3f).SetLink(gameObject);
        }
    }
}