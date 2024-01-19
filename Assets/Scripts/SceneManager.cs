using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Takechi.Scene
{
    public class SceneManager : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        }
    }
}