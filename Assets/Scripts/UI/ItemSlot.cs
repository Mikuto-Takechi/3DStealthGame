using UnityEngine;
using UnityEngine.UI;

namespace MonstersDomain
{
    public class ItemSlot : MonoBehaviour
    {
        [SerializeField] Sprite _notContainsImage;
        [SerializeField] Image _icon;
        [SerializeField] Text _label;
        [SerializeField] Image _rightEdge;
        [SerializeField] Image _leftEdge;

        void Awake()
        {
            _rightEdge.enabled = false;
            _leftEdge.enabled = false;
        }

        public void Set(ItemData itemData, bool isRightEdge = false, bool isLeftEdge = false)
        {
            _icon.sprite = itemData.Icon;
            _label.text = itemData.DisplayName;
            _rightEdge.enabled = isRightEdge;
            _leftEdge.enabled = isLeftEdge;
        }

        public void NotContains()
        {
            _icon.sprite = _notContainsImage;
            _label.text = string.Empty;
            _rightEdge.enabled = false;
            _leftEdge.enabled = false;
        }
    }
}
