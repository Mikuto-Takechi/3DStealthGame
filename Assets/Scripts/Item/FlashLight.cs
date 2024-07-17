using UnityEngine;

namespace MonstersDomain
{
    public class FlashLight : EquippedItem, IUsable
    {
        [SerializeField] Light _light;
        [SerializeField] float _intensity = 10f;
        [SerializeField] GameObject _volumetricLight;
        bool _isPaused;
        bool _toggleLight;

        void Awake()
        {
            _volumetricLight.SetActive(false);
            _light.intensity = 0;
        }

        void Start()
        {
            GameManager.Instance.OnPause += OnPause;
            GameManager.Instance.OnResume += OnResume;
        }

        void Update()
        {
            if (_isPaused) return;
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

        void OnDisable()
        {
            GameManager.Instance.OnPause -= OnPause;
            GameManager.Instance.OnResume -= OnResume;
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

        void OnPause()
        {
            _isPaused = true;
        }

        void OnResume()
        {
            _isPaused = false;
        }
    }
}