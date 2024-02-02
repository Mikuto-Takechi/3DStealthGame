using System;
using System.Collections.Generic;
using UnityEngine;

namespace  MonstersDomain
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Item Data")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] ItemId _id;
        [SerializeField] string _displayName;
        [SerializeField] Sprite _icon;
        [SerializeField] InstancedItem _equipmentItem;
        [SerializeField] DroppedItem _droppedItem;
        [SerializeField] List<ItemParameter> _defaultParametersList;
        public ItemId Id => _id;
        public string DisplayName => _displayName;
        public Sprite Icon => _icon;
        public InstancedItem EquipmentItem => _equipmentItem;
        public DroppedItem DroppedItem => _droppedItem;
        public List<ItemParameter> DefaultParametersList => _defaultParametersList;
    }
    [Serializable]
    public struct ItemParameter : IEquatable<ItemParameter>
    {
        public ParameterId ID;
        public float Value;
        public bool Equals(ItemParameter other)
        {
            return other.ID == ID;
        }
    }
}