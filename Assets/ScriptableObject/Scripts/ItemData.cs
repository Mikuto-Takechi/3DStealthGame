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
        [SerializeField] EquippedItem _equipmentItem;
        [SerializeField] DroppedItem _droppedItem;
        [SerializeField] List<ItemParameter> _defaultParameters;
        [SerializeField] List<DisplayParameter> _displayParameters;
        public ItemId Id => _id;
        public string DisplayName => _displayName;
        public Sprite Icon => _icon;
        public EquippedItem EquipmentItem => _equipmentItem;
        public DroppedItem DroppedItem => _droppedItem;
        public List<ItemParameter> DefaultParameters => _defaultParameters;
        public List<DisplayParameter> DisplayParameters => _displayParameters;
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

    [Serializable]
    public struct DisplayParameter
    {
        public ParameterId ID;
        public ParameterUI UI;
    }
}