using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonBase<AudioManager>
{
    [SerializeField] RandomClip _footSteps;
    AudioSource _seSource, _bgmSource;
    protected override void AwakeFunction()
    {
        _seSource = gameObject.AddComponent<AudioSource>();
        _bgmSource = gameObject.AddComponent<AudioSource>();
    }
    public void PlayFootStep()
    {
        _seSource.PlayOneShot(_footSteps.Choice());
    }
}
[Serializable]
public struct RandomClip
{
    [SerializeField] AudioClip[] _clips;
    public AudioClip Choice()
    {
        return _clips[UnityEngine.Random.Range(0, _clips.Length - 1)];
    }
}
