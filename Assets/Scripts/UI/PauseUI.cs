using System;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MonstersDomain
{
    public class PauseUI : MonoBehaviour
    {
        [SerializeField] List<PauseTweenGroup> _pauseTweenGroups;
        [SerializeField] Button _settingsButton;
        [SerializeField] Button _titleButton;
        [SerializeField] Button _quitButton;
        List<IDisposable> _subscriptions = new();
        CanvasGroup _canvasGroup;
        Sequence _sequence;

        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 0;
            for (int i = 0; i < _pauseTweenGroups.Count; i++)
            {
                if (_pauseTweenGroups[i].InitialAnchoredPosition == null)
                {
                    var pauseTweenGroup = _pauseTweenGroups[i];
                    pauseTweenGroup.InitialAnchoredPosition = new();
                    _pauseTweenGroups[i] = pauseTweenGroup;
                }

                foreach (var t in _pauseTweenGroups[i].Elements)
                {
                    _pauseTweenGroups[i].InitialAnchoredPosition.Add(t.anchoredPosition);
                }
            }
        }

        void Register()
        {
            _subscriptions.Add(_settingsButton.OnClickAsObservable().Subscribe(_=>print("設定")).AddTo(this));
            _subscriptions.Add(_titleButton.OnClickAsObservable()
                .Subscribe(_ => SceneManager.LoadSceneFade("Title",()=> GameManager.Instance.CurrentGameState.Value = GameState.Title)).AddTo(this));
            _subscriptions.Add(_quitButton.OnClickAsObservable().Subscribe(_ =>
            {
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
                #else
                Application.Quit();//ゲームプレイ終了
                #endif
            }).AddTo(this));
        }

        void DeRegister()
        {
            _subscriptions?.ForEach(d=>d?.Dispose());
            _subscriptions.Clear();
        }

        public void ForceHidden(ref bool isPaused)
        {
            isPaused = false;
            DeRegister();
            _sequence?.Kill();
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 0;
        }

        public void Pause(Action callback)
        {
            if (_sequence != null) return;
            Register();
            foreach (var pauseTweenGroup in _pauseTweenGroups)
            foreach (var element in pauseTweenGroup.Elements)
            {
                var temp = element.anchoredPosition;
                temp.x -= Screen.width / 3;
                element.anchoredPosition = temp;
            }
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 1;
            _sequence = DOTween.Sequence().OnComplete(()=>
            {
                _sequence = null;
                callback?.Invoke();
            }).SetLink(gameObject);
            for (int i = _pauseTweenGroups.Count - 1; i >= 0; i--)
            {
                for (int j = _pauseTweenGroups[i].Elements.Count - 1; j >= 0; j--)
                {
                    _sequence.Join(_pauseTweenGroups[i].Elements[j]
                        .DOAnchorPosX(_pauseTweenGroups[i].InitialAnchoredPosition[j].x, 0.5f));
                }
                _sequence.SetDelay(0.1f);
            }
        }
        public void Resume(Action callback)
        {
            if (_sequence != null) return;
            DeRegister();
            _sequence = DOTween.Sequence().OnComplete(()=>
            {
                _sequence = null;
                callback?.Invoke();
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.alpha = 0;
                foreach (var t in _pauseTweenGroups)
                {
                    for (int j = 0; j < t.Elements.Count; j++)
                    {
                        t.Elements[j].anchoredPosition =
                            t.InitialAnchoredPosition[j];
                    }
                }
            }).SetLink(gameObject);
            foreach (var t in _pauseTweenGroups)
            {
                for (int j = 0; j < t.Elements.Count; j++)
                {
                    var offset = t.InitialAnchoredPosition[j].x - Screen.width / 3;
                    _sequence.Join(t.Elements[j]
                        .DOAnchorPosX(offset, 0.5f));
                }
                _sequence.SetDelay(0.1f);
            }
        }
    }

    [Serializable]
    public struct PauseTweenGroup
    {
        [field : SerializeField]
        public List<RectTransform> Elements { get; set; }

        public List<Vector2> InitialAnchoredPosition { get; set; }
    }
}