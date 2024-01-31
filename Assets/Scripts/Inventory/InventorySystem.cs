using System.Collections.Generic;
using UnityEngine;

namespace MonstersDomain
{
    public class InventorySystem : MonoBehaviour
    {
        [SerializeField] ItemDataBase _itemDataBase;
        List<ItemId> _itemContainer = new();
        /// <summary>インベントリにアイテムを追加する</summary>
        public void Add(ItemData data)
        {
            _itemContainer.Add(data.Id);
        }

        /// <summary>インベントリからアイテムを取り出す</summary>
        public void Drop(int index)
        {
            Instantiate(_itemDataBase[_itemContainer[index]].FieldPrefab);
            _itemContainer.RemoveAt(index);
        }

        /// <summary>インベントリのアイテムを手に装備する</summary>
        public void Equip(int index)
        {
            Instantiate(_itemDataBase[_itemContainer[index]].EquipmentPrefab);
        }
    }
}
