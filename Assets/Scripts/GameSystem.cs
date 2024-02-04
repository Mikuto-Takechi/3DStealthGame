using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonstersDomain
{
    public class GameSystem : MonoBehaviour
    {
        public void LoadGame(string sceneName)
        {
            SceneManager.LoadSceneFade(sceneName, () => GameManager.Instance.CurrentGameState.Value = GameState.InGame);
        }
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadSceneFade(sceneName);
        }
    }
}
