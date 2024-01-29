using UnityEngine;

namespace MonstersDomain
{
    public class FlashLight : MonoBehaviour, IUsable
    {
        [SerializeField] private Light _light;
        [SerializeField] private float _intensity = 10f;

        private void Awake()
        {
            _light.intensity = 0;
        }

        public void Use()
        {
            if (_light.intensity <= 0)
            {
                _light.intensity = _intensity;
                AudioManager.Instance.PlaySE(SE.FlashLightOn);
            }
            else
            {
                _light.intensity = 0;
                AudioManager.Instance.PlaySE(SE.FlashLightOff);
            }
        }
    }
}
