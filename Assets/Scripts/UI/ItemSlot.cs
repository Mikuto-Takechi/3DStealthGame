using UnityEngine;
using UnityEngine.UI;

namespace MonstersDomain
{
    public class ItemSlot : MonoBehaviour
    {
        [SerializeField] Image _icon;
        [SerializeField] Text _label;
        
        public void Set(InventoryItemData inventoryItemData)
        {
            _icon.sprite = inventoryItemData.Icon;
            _label.text = inventoryItemData.DisplayName;
        }
    }
}
