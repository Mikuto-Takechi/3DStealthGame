using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MonstersDomain
{
    /// <summary>
    /// シーンマネージャーに機能を追加
    /// </summary>
    public class SceneManager : UnityEngine.SceneManagement.SceneManager
    {
        static Tween _fadeTween;
        static Image _fadePanel;
        static Sprite _loadSceneSprite;
        const string LoadSceneSpritePath = "Images/LoadSceneSprite";
        const string FadeCanvasPath = "Prefabs/FadeCanvas";
        static readonly int Alpha = Shader.PropertyToID("_Alpha");
        public static void LoadSceneFade(string sceneName, Action callback = null)
        {
            if (_fadeTween != null) return;
            _fadePanel.sprite = _loadSceneSprite;
            _fadeTween = DOVirtual.Float(0, 1, 1, value => _fadePanel.material.SetFloat(Alpha, value));
            _fadeTween.OnComplete(() =>
            {
                var asyncOperation = LoadSceneAsync(sceneName);
                asyncOperation.completed += _ =>
                {
                    callback?.Invoke();
                    DOVirtual.Float(1, 0, 1, value => _fadePanel.material.SetFloat(Alpha, value))
                        .OnComplete(() => _fadeTween = null);
                };
            });
        }
        static SceneManager()
        {
            _loadSceneSprite = Resources.Load<Sprite>(LoadSceneSpritePath);
            var fadeCanvas = Object.Instantiate(Resources.Load<GameObject>(FadeCanvasPath));
            Object.DontDestroyOnLoad(fadeCanvas);
            _fadePanel = fadeCanvas.transform.GetComponentInChildren<Image>();
        }
    }
}