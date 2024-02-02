using System;
using UnityEngine;

namespace MonstersDomain
{
    public class FlashLight : EquippedItem, IUsable
    {
        [SerializeField] Light _light;
        [SerializeField] float _intensity = 10f;
        [SerializeField] GameObject _volumetricLight;
        bool _toggleLight = false;

        void Awake()
        {
            _volumetricLight.SetActive(false);
            _light.intensity = 0;
        }

        public void Use()
        {
            //  パラメーターが足りなければ使えなくする。
            if (!RequiredParameters()) return;
            if (!_toggleLight)
            {
                _toggleLight = true;
                _volumetricLight.SetActive(true);
                _light.intensity = _intensity;
                AudioManager.Instance.PlaySE(SE.FlashLightOn);
            }
            else
            {
                _toggleLight = false;
                _volumetricLight.SetActive(false);
                _light.intensity = 0;
                AudioManager.Instance.PlaySE(SE.FlashLightOff);
            }
        }

        void Update()
        {
            if (_toggleLight && RequiredParameters())
            {
                ModifyParameters(Time.deltaTime);
            }
            else
            {
                _toggleLight = false;
                _volumetricLight.SetActive(false);
                _light.intensity = 0;
            }
        }
    }
}
