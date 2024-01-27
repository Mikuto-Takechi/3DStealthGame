using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonstersDomain
{
    public class SceneManager : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        }
    }
}