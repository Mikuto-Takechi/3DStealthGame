using UnityEngine;

namespace MonstersDomain
{
    public class Pickable : MonoBehaviour
    {
        [SerializeField] ItemId _itemId;
        public void PickUp()
        {
            print("拾った");
            InteractionMessage.Instance.WriteText("");
            ItemManager.Instance.Add(_itemId);
            Destroy(gameObject);
        }
    }
}
