using UnityEngine;
using UnityEngine.UI;

namespace MonstersDomain
{
    public class ItemSlot : MonoBehaviour
    {
        [SerializeField] Image _icon;
        [SerializeField] Text _label;
        
        public void Set(ItemData itemData)
        {
            _icon.sprite = itemData.Icon;
            _label.text = itemData.DisplayName;
        }
    }
}
