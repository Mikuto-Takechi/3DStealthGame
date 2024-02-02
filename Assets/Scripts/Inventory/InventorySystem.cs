using System.Collections.Generic;
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
        [SerializeField] Transform _parameterUIAnchor;
        protected ReactiveCollection<(ItemId, List<ItemParameter>)> ItemContainer { get; } = new();
        protected EquippedItem EquippedItem { get; private set; }
        protected abstract void OnDrop();

        /// <summary>インベントリにアイテムを追加する</summary>
        public void Add(ItemId id, List<ItemParameter> param = null) => ItemContainer.Add((id, param));

        /// <summary>インベントリからアイテムを取り出す</summary>
        public void Drop(int index)
        {
            if (ItemContainer.Count <= 0 || !ArrayUtil.CheckIndexOutOfRange(ItemContainer, index)) return;
            var obj =  Instantiate(_itemDataBase[ItemContainer[index].Item1].DroppedItem);
            if (_itemDataBase[ItemContainer[index].Item1].DefaultParameters.Count > 0)
            {
                obj.InheritParameters(ItemContainer[index].Item2);
            }
            obj.transform.position = _dropAnchor.position;
            ItemContainer.RemoveAt(index);
            OnDrop();
        }

        /// <summary>インベントリのアイテムを手に装備する</summary>
        protected void Equipment(int index)
        {
            if (ItemContainer.Count <= 0 || !ArrayUtil.CheckIndexOutOfRange(ItemContainer, index)) return;
            EquippedItem = Instantiate(_itemDataBase[ItemContainer[index].Item1].EquipmentItem, _equipmentAnchor);
            EquippedItem.InheritParameters(ItemContainer[index].Item2, true, _parameterUIAnchor);
        }

        /// <summary>手に持っているアイテムを空にする</summary>
        protected void UnEquipment()
        {
            if (EquippedItem)
            {
                if (EquippedItem.ParametersUI.Count > 0)
                {
                    foreach (var parameterUI in EquippedItem.ParametersUI)
                        Destroy(parameterUI.UI.gameObject);
                }
                Destroy(EquippedItem.gameObject);
                EquippedItem = null;
            }
        }
    }
}