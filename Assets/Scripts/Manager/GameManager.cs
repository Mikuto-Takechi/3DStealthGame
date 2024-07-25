using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

namespace MonstersDomain
{
    public enum GameState
    {
        Title,
        InGame,
        GameClear,
        GameOver
    }

    public class GameManager : SingletonBase<GameManager>
    {
        const string PauseCanvasPath = "Prefabs/PauseCanvas";
        const string GlobalVolumePath = "Prefabs/GlobalVolume";
        [SerializeField] List<string> _inGameScenes;
        ChromaticAberration _chromatic;
        DepthOfField _depthOfField;
        float _inGameTimer;
        bool _isPaused;
        PauseUI _pauseUI;
        Vignette _vignette;
        PostProcessVolume _volume;
        public ReactiveProperty<GameState> CurrentGameState { get; set; } = new(GameState.Title);
        public Action OnPause { get; set; }
        public Action OnResume { get; set; }

        void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void Pause()
        {
            CreatePauseCanvas();
            InputProvider.Instance.CurrentBindInput &= ~(ActionType.Move | ActionType.Crouch | ActionType.Drop
                                                         | ActionType.Jump | ActionType.Run | ActionType.Dance
                                                         | ActionType.Interact | ActionType.Use |
                                                         ActionType.SelectHotbar);
            Cursor.lockState = CursorLockMode.None;
            OnPause?.Invoke();
            _depthOfField.enabled.value = true;
            _pauseUI.Pause(() => _isPaused = true);
        }

        void Resume()
        {
            InputProvider.Instance.CurrentBindInput |= ActionType.Move | ActionType.Crouch | ActionType.Drop
                                                       | ActionType.Jump | ActionType.Run | ActionType.Dance
                                                       | ActionType.Interact | ActionType.Use | ActionType.SelectHotbar;
            Cursor.lockState = CursorLockMode.Locked;
            OnResume?.Invoke();
            _depthOfField.enabled.value = false;
            _pauseUI.Resume(() => _isPaused = false);
        }

        void CreatePauseCanvas()
        {
            if (_pauseUI != null) return;
            _pauseUI = Instantiate(Resources.Load<PauseUI>(PauseCanvasPath));
            DontDestroyOnLoad(_pauseUI.gameObject);
        }

        void GameClear()
        {
            SceneManager.LoadSceneFade("GameClear");
        }

        void GameOver()
        {
            SceneManager.LoadSceneFade("GameOver");
        }

        public void ChasePostEffect(bool flag)
        {
            _chromatic.enabled.value = flag;
            _vignette.enabled.value = flag;
        }

        protected override void AwakeFunction()
        {
            CurrentGameState.SkipLatestValueOnSubscribe().Where(state => state == GameState.GameClear)
                .Subscribe(_ => GameClear())
                .AddTo(this);
            CurrentGameState.SkipLatestValueOnSubscribe().Where(state => state == GameState.GameOver)
                .Subscribe(_ => GameOver())
                .AddTo(this);
            InputProvider.Instance.PauseTrigger.Subscribe(_ =>
            {
                if (_isPaused)
                    Resume();
                else
                    Pause();
            }).AddTo(this);
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            _volume = Instantiate(Resources.Load<PostProcessVolume>(GlobalVolumePath));
            DontDestroyOnLoad(_volume.gameObject);
            _volume.profile.TryGetSettings(out _chromatic);
            _volume.profile.TryGetSettings(out _vignette);
            _volume.profile.TryGetSettings(out _depthOfField);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_inGameScenes.Any(str => str == scene.name))
            {
                Cursor.lockState = CursorLockMode.Locked;
                _inGameTimer = 0;
                _isPaused = false;
                _depthOfField.enabled.value = false;
                _vignette.enabled.value = false;
                _chromatic.enabled.value = false;
                InputProvider.Instance.CurrentBindInput |= InputProvider.Instance.AllActionType;
            }
            else
            {
                InputProvider.Instance.CurrentBindInput &= ~ActionType.Pause;
                CreatePauseCanvas();
                _pauseUI.ForceHidden(ref _isPaused);
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
}