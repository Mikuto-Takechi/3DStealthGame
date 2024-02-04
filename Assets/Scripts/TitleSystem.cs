using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonstersDomain
{
    public class TitleSystem : MonoBehaviour
    {
        public void StartGame(string sceneName)
        {
            GameManager.Instance.GameStart(sceneName);
        }
    }
}
