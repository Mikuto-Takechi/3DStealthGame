using UnityEngine;

namespace MonstersDomain
{
    public class ItemTester : MonoBehaviour
    {
        [SerializeField] ItemId _itemId;
        [ContextMenu("AddTest")]
        void AddTest()
        {
            ItemManager.Instance.Add(_itemId);
        }
        [ContextMenu("DropTest")]
        void DropTest()
        {
            ItemManager.Instance.Drop(0);
        }
    }
}
