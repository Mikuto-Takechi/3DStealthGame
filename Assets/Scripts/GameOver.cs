using System;
using UniRx;
using UnityEngine;

namespace MonstersDomain
{
    /// <summary>
    /// ゲームオーバー時の演出処理を行うクラス。
    /// </summary>
    public class GameOver : MonoBehaviour
    {
        [SerializeField] ParasiteEventDispatcher _dispatcher;
        IDisposable _disposable;

        void Start()
        {
            _disposable = _dispatcher.EventFootSteps
                .Subscribe(_ =>
                    AudioManager.Instance.Play3DFootSteps(FootSteps.Parasite, _dispatcher.transform.position))
                .AddTo(this);
        }

        public void StopFootSteps()
        {
            _disposable?.Dispose();
        }
    }
}