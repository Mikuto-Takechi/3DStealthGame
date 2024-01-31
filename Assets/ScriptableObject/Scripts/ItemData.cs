using UnityEngine;

namespace  MonstersDomain
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Item Data")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] ItemId _id;
        [SerializeField] string _displayName;
        [SerializeField] Sprite _icon;
        [SerializeField] GameObject _equipmentPrefab;
        [SerializeField] GameObject _fieldPrefab;
        public ItemId Id => _id;
        public string DisplayName => _displayName;
        public Sprite Icon => _icon;
        public GameObject EquipmentPrefab => _equipmentPrefab;
        public GameObject FieldPrefab => _fieldPrefab;
    }
}