using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MonstersDomain
{
    public class GameOver : MonoBehaviour
    {
        [SerializeField] ParasiteEventDispatcher _dispatcher;
        IDisposable _disposable;

        void Start()
        {
            _disposable = _dispatcher.EventFootSteps
                .Subscribe(_=>AudioManager.Instance.Play3DFootSteps(FootSteps.Parasite, _dispatcher.transform.position)).AddTo(this);
        }

        public void StopFootSteps()
        {
            _disposable?.Dispose();
        }
    }
}
