using UniRx;
using UnityEngine;

namespace MonstersDomain
{
    public abstract class InventorySystem : MonoBehaviour
    {
        [SerializeField] ItemDataBase _itemDataBase;
        [SerializeField] Transform _equipmentAnchor;
        protected ReactiveCollection<ItemId> ItemContainer { get; } = new();
        protected GameObject EquippedItem { get; private set; }

        /// <summary>インベントリにアイテムを追加する</summary>
        public void Add(ItemId id) => ItemContainer.Add(id);

        /// <summary>インベントリからアイテムを取り出す</summary>
        public void Drop(int index)
        {
            Instantiate(_itemDataBase[ItemContainer[index]].FieldPrefab);
            ItemContainer.RemoveAt(index);
        }

        /// <summary>インベントリのアイテムを手に装備する</summary>
        protected void Equipment(int index)
        {
            if (ItemContainer.Count <= 0) return;
            EquippedItem = Instantiate(_itemDataBase[ItemContainer[index]].EquipmentPrefab, _equipmentAnchor);
        }

        /// <summary>手に持っているアイテムを空にする</summary>
        protected void UnEquipment()
        {
            if (EquippedItem)
            {
                Destroy(EquippedItem);
                EquippedItem = null;
            }
        }
    }
}