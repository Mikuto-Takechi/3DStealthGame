using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
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
        public ReactiveProperty<GameState>  CurrentGameState { get; set; } = new(GameState.Title);
        bool _cursorLock = false;
        void GameClear()
        {
            SceneManager.LoadSceneFade("GameClear");
        }

        void GameOver()
        {
            SceneManager.LoadSceneFade("GameOver");
        }

        protected override void AwakeFunction()
        {
            CurrentGameState.SkipLatestValueOnSubscribe().Where(state => state == GameState.GameClear)
                .Subscribe(_=> GameClear())
                .AddTo(this);
            CurrentGameState.SkipLatestValueOnSubscribe().Where(state => state == GameState.GameOver)
                .Subscribe(_=> GameOver())
                .AddTo(this);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "InGame")
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
}
