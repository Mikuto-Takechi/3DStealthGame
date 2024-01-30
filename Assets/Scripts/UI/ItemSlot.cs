using UnityEngine;
using UnityEngine.UI;

namespace MonstersDomain
{
    public class ItemSlot : MonoBehaviour
    {
        [SerializeField] Image _icon;
        [SerializeField] Text _label;
        
        public void Set(Item item)
        {
            _icon.sprite = item.Icon;
            _label.text = item.Name;
        }
    }
}
