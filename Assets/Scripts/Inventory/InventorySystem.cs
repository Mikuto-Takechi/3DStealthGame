﻿using System.Collections.Generic;
using System.Linq;
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

        public ReactiveCollection<(ItemId, List<ItemParameter>)> ItemContainer { get; } =
            new( /*Enumerable.Repeat<(ItemId, List<ItemParameter>)>((ItemId.Nothing, null), 5).ToList()*/);

        protected EquippedItem EquippedItem { get; private set; }
        protected abstract void OnDrop();

        /// <summary>インベントリにアイテムを追加する</summary>
        public void Add(ItemId id, List<ItemParameter> param = null)
        {
            ItemContainer.Add((id, param));
        }

        /// <summary>インベントリからアイテムを取り出す</summary>
        public void Drop(int index)
        {
            if (ItemContainer.Count <= 0 || !ArrayUtil.CheckIndexOutOfRange(ItemContainer, index)) return;
            var obj = Instantiate(_itemDataBase[ItemContainer[index].Item1].DroppedItem);
            if (_itemDataBase[ItemContainer[index].Item1].DefaultParameters.Count > 0)
                obj.InheritParameters(ItemContainer[index].Item2);
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
                    foreach (var parameterUI in EquippedItem.ParametersUI)
                        Destroy(parameterUI.UI.gameObject);
                Destroy(EquippedItem.gameObject);
                EquippedItem = null;
            }
        }

        public bool RequestItem(ItemId itemId)
        {
            if (ItemContainer.Select(i => i.Item1).Contains(itemId))
            {
                ItemContainer.RemoveAt(ItemContainer
                    .Select((item, index) => (item.Item1, index)).First(item => item.Item1 == itemId).Item2);
                return true;
            }

            return false;
        }
    }
}