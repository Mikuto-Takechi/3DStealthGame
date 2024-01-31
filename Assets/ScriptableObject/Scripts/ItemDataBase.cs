using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MonstersDomain
{
    [CreateAssetMenu(menuName = "ScriptableObjects/ItemDataBase")]
    public class ItemDataBase : ScriptableObject
    {
        [SerializeField] List<ItemData> _items = new();
        public ItemData this[ItemId key]
        {
            get
            {
                return _items.FirstOrDefault(itemData => itemData.Id == key);
            }
        }
    }
}