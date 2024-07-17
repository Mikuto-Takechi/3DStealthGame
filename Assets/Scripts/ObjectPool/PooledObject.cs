using System;
using System.Collections;
using UnityEngine;

namespace MonstersDomain
{
    public class PooledObject : MonoBehaviour
    {
        public ObjectPool Pool { get; set; }

        public void Release()
        {
            Pool.ReturnToPool(this);
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