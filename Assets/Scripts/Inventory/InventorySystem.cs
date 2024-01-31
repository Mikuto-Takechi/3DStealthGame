using MonstersDomain.Common;
using UniRx;
using UnityEngine;

namespace MonstersDomain
{
    public abstract class InventorySystem : MonoBehaviour
    {
        [SerializeField] ItemDataBase _itemDataBase;
        [SerializeField] Transform _equipmentAnchor;
        [SerializeField] Transform _dropAnchor;
        protected ReactiveCollection<ItemId> ItemContainer { get; } = new();
        protected GameObject EquippedItem { get; private set; }
        protected abstract void OnDrop();

        /// <summary>インベントリにアイテムを追加する</summary>
        public void Add(ItemId id) => ItemContainer.Add(id);

        /// <summary>インベントリからアイテムを取り出す</summary>
        public void Drop(int index)
        {
            if (ItemContainer.Count <= 0 || !ArrayUtil.CheckIndexOutOfRange(ItemContainer, index)) return;
            var obj =  Instantiate(_itemDataBase[ItemContainer[index]].FieldPrefab);
            obj.transform.position = _dropAnchor.position;
            ItemContainer.RemoveAt(index);
            OnDrop();
        }

        /// <summary>インベントリのアイテムを手に装備する</summary>
        protected void Equipment(int index)
        {
            if (ItemContainer.Count <= 0 || !ArrayUtil.CheckIndexOutOfRange(ItemContainer, index)) return;
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