using UnityEngine;

namespace MonstersDomain
{
    public class Pickable : MonoBehaviour
    {
        [SerializeField] ItemId _itemId;
        public void PickUp()
        {
            AudioManager.Instance.PlaySE(SE.Drop);
            InteractionMessage.Instance.WriteText("");
            ItemManager.Instance.Add(_itemId);
            Destroy(gameObject);
        }
    }
}
