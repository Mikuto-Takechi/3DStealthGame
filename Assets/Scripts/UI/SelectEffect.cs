using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MonstersDomain
{
    public class SelectEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            AudioManager.Instance.PlaySE(SE.ButtonSelect);
            transform.DOScale(1.2f, 0.3f).SetLink(gameObject);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DOScale(1, 0.3f).SetLink(gameObject);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            AudioManager.Instance.PlaySE(SE.ButtonSubmit);
        }
    }
}
