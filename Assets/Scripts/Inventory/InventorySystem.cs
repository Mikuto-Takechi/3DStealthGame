using System.Collections.Generic;
using UnityEngine;

namespace MonstersDomain
{
    public class InventorySystem : MonoBehaviour
    {
        [SerializeField] ItemDataBase _itemDataBase;
        List<ItemId> _itemList = new();
        /// <summary>インベントリにアイテムを追加する</summary>
        public void Add(InventoryItemData data)
        {
            _itemList.Add(data.Id);
        }

        /// <summary>インベントリからアイテムを取り出す</summary>
        public void Drop(int index)
        {
            Instantiate(_itemDataBase.ItemDic[_itemList[index]].FieldPrefab);
            _itemList.RemoveAt(index);
        }

        /// <summary>インベントリのアイテムを手に装備する</summary>
        public void Equip(int index)
        {
            Instantiate(_itemDataBase.ItemDic[_itemList[index]].EquipmentPrefab);
        }
    }
}
