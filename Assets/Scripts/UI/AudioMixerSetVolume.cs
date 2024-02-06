using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace MonstersDomain
{
    public class AudioMixerSetVolume : MonoBehaviour
    {
        [SerializeField] AudioMixer _audioMixer;
        [SerializeField] Slider _masterSlider;
        [SerializeField] Slider _seSlider;
        [SerializeField] Slider _bgmSlider;
        [SerializeField] Slider _ambientSlider;
        /// <summary>デシベルの最大値</summary>
        static readonly float MAX_DECIBEL = 0;
        /// <summary>デシベルの最小値</summary>
        static readonly float MIN_DECIBEL = -80f;

        void Start()
        {
            if (_audioMixer.GetFloat("MasterVolume", out float masterDb))
                _masterSlider.value = FromDecibel(masterDb);
            if(_audioMixer.GetFloat("SEVolume", out float seDb))
                _seSlider.value = FromDecibel(seDb);
            if(_audioMixer.GetFloat("BGMVolume", out float bgmDb))
                _bgmSlider.value = FromDecibel(bgmDb);
            if(_audioMixer.GetFloat("AmbientVolume", out float ambientDb))
                _ambientSlider.value = FromDecibel(ambientDb);
            
            _masterSlider.onValueChanged.AddListener(SetMasterVolume);
            _seSlider.onValueChanged.AddListener(SetSeVolume);
            _bgmSlider.onValueChanged.AddListener(SetBgmVolume);
            _ambientSlider.onValueChanged.AddListener(SetAmbientVolume);
        }
        /// <summary>
        /// 数値をデシベルへ変換する
        /// </summary>
        float ToDecibel(float linear)
        {
            return Mathf.Clamp(Mathf.Log10(Mathf.Clamp(linear, 0f, 1f)) * 20f, MIN_DECIBEL, MAX_DECIBEL);
        }
        /// <summary>
        /// デシベルから数値へ変換する
        /// </summary>
        float FromDecibel(float decibel)
        {
            return Mathf.Pow(10f, decibel / 20f);
        }

        public void SetMasterVolume(float volume) => _audioMixer.SetFloat("MasterVolume",ToDecibel(_masterSlider.value));
        public void SetSeVolume(float volume) => _audioMixer.SetFloat("SEVolume",ToDecibel(_seSlider.value));
        public void SetBgmVolume(float volume) => _audioMixer.SetFloat("BGMVolume",ToDecibel(_bgmSlider.value));
        public void SetAmbientVolume(float volume) => _audioMixer.SetFloat("AmbientVolume",ToDecibel(_ambientSlider.value));
    }
}
