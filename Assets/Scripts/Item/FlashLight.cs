using UnityEngine;

namespace MonstersDomain
{
    public class FlashLight : MonoBehaviour, IUsable
    {
        [SerializeField] Light _light;
        [SerializeField] float _intensity = 10f;
        [SerializeField] GameObject _volumetricLight;

        void Awake()
        {
            _volumetricLight.SetActive(false);
            _light.intensity = 0;
        }

        public void Use()
        {
            if (_light.intensity <= 0)
            {
                _volumetricLight.SetActive(true);
                _light.intensity = _intensity;
                AudioManager.Instance.PlaySE(SE.FlashLightOn);
            }
            else
            {
                _volumetricLight.SetActive(false);
                _light.intensity = 0;
                AudioManager.Instance.PlaySE(SE.FlashLightOff);
            }
        }
    }
}
