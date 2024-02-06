using System;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

namespace MonstersDomain
{
    public class AudioManager : SingletonBase<AudioManager>
    {
        [SerializeField] AudioDataBase _musicData;
        [SerializeField] AudioDataBase _ambientData;
        [SerializeField] AudioDataBase _seData;
        [SerializeField] AudioGroupDataBase _footStepsData;
        [SerializeField] ObjectPool _audioSourcePool;
        [SerializeField] AudioMixerGroup[] _audioMixerGroups;
        AudioSource _seSource, _bgmSource, _ambientSource, _footStepsSource;

        protected override void AwakeFunction()
        {
            _seSource = gameObject.AddComponent<AudioSource>();
            _seSource.playOnAwake = false;
            _seSource.outputAudioMixerGroup = _audioMixerGroups[0];
            _footStepsSource = gameObject.AddComponent<AudioSource>();
            _footStepsSource.playOnAwake = false;
            _footStepsSource.outputAudioMixerGroup = _audioMixerGroups[0];
            _bgmSource = gameObject.AddComponent<AudioSource>();
            _bgmSource.playOnAwake = false;
            _bgmSource.loop = true;
            _bgmSource.outputAudioMixerGroup = _audioMixerGroups[1];
            _ambientSource = gameObject.AddComponent<AudioSource>();
            _ambientSource.playOnAwake = false;
            _ambientSource.loop = true;
            _ambientSource.outputAudioMixerGroup = _audioMixerGroups[2];
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StopAmbient();
            StopMusic();
        }

        public void PlayFootSteps(FootSteps footSteps, float pitch, float volume)
        {
            var clips = _footStepsData.AudioGroups[(int)footSteps].Clips;
            _footStepsSource.pitch = pitch;
            _footStepsSource.volume = volume;
            _footStepsSource.PlayOneShot(clips[Random.Range(0, clips.Length - 1)]);
        }

        public void Play3DFootSteps(FootSteps footSteps, Vector3 point)
        {
            var clips = _footStepsData.AudioGroups[(int)footSteps].Clips;
            Play3DSound(clips[Random.Range(0, clips.Length - 1)], point, 1);
        }

        public void PlaySE(SE se)
        {
            _seSource.PlayOneShot(_seData.Clips[(int)se].Clip);
        }

        public void Play3DSE(SE se, Vector3 point)
        {
            Play3DSound(_seData.Clips[(int)se].Clip, point, 1);
        }

        public void PlayMusic(Music music)
        {
            _bgmSource.clip = _musicData.Clips[(int)music].Clip;
            _bgmSource.Play();
        }

        public void PlayAmbient(Ambient ambient)
        {
            _ambientSource.clip = _ambientData.Clips[(int)ambient].Clip;
            _ambientSource.Play();
        }

        public void StopMusic()
        {
            _bgmSource.Stop();
        }
        public void StopAmbient()
        {
            _ambientSource.Stop();
        }

        void Play3DSound(AudioClip clip, Vector3 point, float volume)
        {
            var pooledObj = _audioSourcePool.GetPooledObject();
            var audioSource = pooledObj.GetComponent<AudioSource>();
            audioSource.transform.position = point;
            audioSource.clip = clip;
            audioSource.spatialBlend = 1f;
            audioSource.volume = volume;
            audioSource.Play();
            pooledObj.Release(clip.length * (Time.timeScale < 0.009999999776482582 ? 0.01f : Time.timeScale));
        }
    }

    [Serializable]
    public struct RandomClip
    {
        [SerializeField] AudioClip[] _clips;

        public AudioClip Choice()
        {
            return _clips[Random.Range(0, _clips.Length - 1)];
        }
    }
}