using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

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

        public void GameStart(string sceneName)
        {
            SceneManager.LoadSceneFade(sceneName, () => CurrentGameState.Value = GameState.InGame);
        }

        void GameClear()
        {
            print("ゲームクリア");
        }

        void GameOver()
        {
            print("ゲームオーバー");
        }

        protected override void AwakeFunction()
        {
            CurrentGameState.SkipLatestValueOnSubscribe().Where(state => state == GameState.GameClear)
                .Subscribe(_=> GameClear())
                .AddTo(this);
            CurrentGameState.SkipLatestValueOnSubscribe().Where(state => state == GameState.GameOver)
                .Subscribe(_=> GameOver())
                .AddTo(this);
        }
    }
}
