using UnityEngine;

public abstract class SingletonBase<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; set; }
    protected abstract void AwakeFunction();
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
}
