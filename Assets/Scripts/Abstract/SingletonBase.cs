using UnityEngine;

namespace MonstersDomain
{
    public abstract class SingletonBase<T> : MonoBehaviour where T : Component
    {
        public static T Instance { get; set; }

        protected void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(gameObject);
                AwakeFunction();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected abstract void AwakeFunction();
    }
}