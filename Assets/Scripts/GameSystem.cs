using UnityEditor;
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

        public void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false; //ゲームプレイ終了
#else
                Application.Quit();//ゲームプレイ終了
#endif
        }
    }
}