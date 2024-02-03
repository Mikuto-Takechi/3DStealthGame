using UnityEngine;
using UnityEngine.UI;

namespace MonstersDomain
{
    public class ParameterUI : MonoBehaviour
    {
        [SerializeField] Image _bar;

        public float Value
        {
            get => _bar.fillAmount;
            set => _bar.fillAmount = value;
        }
    }
}
