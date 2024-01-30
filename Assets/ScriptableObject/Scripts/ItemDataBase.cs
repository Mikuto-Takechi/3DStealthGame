using System.Collections.Generic;
using UnityEngine;

namespace MonstersDomain
{
    [CreateAssetMenu(fileName = "ItemDataBase", menuName = "ScriptableObjects/ItemDataBase", order = 1)]
    public class ItemDataBase : ScriptableObject
    {
        public InventoryItemData[] Items;
        Dictionary<ItemId, InventoryItemData> _itemDic = new();
        public Dictionary<ItemId, InventoryItemData> ItemDic => _itemDic;
        ItemDataBase()
        {
            foreach (var item in Items)
            {
                _itemDic.Add(item.Id, item);
            }
        }
    }
}