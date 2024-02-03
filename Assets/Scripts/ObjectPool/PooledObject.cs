using System;
using System.Collections;
using UnityEngine;

namespace MonstersDomain
{
    public class PooledObject : MonoBehaviour
    {
        ObjectPool _pool;
        public ObjectPool Pool { get => _pool; set => _pool = value; }

        public void Release()
        {
            _pool.ReturnToPool(this);
        }
        public void Release(float t)
        {
            StartCoroutine(Delay(t, Release));
        }

        IEnumerator Delay(float t, Action callback)
        {
            float time = 0;
            while (true)
            {
                time += Time.deltaTime;
                if (time > t)
                {
                    callback?.Invoke();
                    yield break;
                }
                yield return null;
            }
        }
    }
}
