using System.Collections.Generic;
using UnityEngine;

namespace MonstersDomain
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] uint _initPoolSize;
        [SerializeField] PooledObject _objectToPool;

        // プールされたオブジェクトをコレクションに格納する
        Stack<PooledObject> _stack;

        void Start()
        {
            SetupPool();
        }

        // プールを作成する（ラグが目立たなくなったら呼び出す）
        void SetupPool()
        {
            _stack = new Stack<PooledObject>();
            PooledObject instance = null;

            for (var i = 0; i < _initPoolSize; i++)
            {
                instance = Instantiate(_objectToPool, transform);
                instance.Pool = this;
                instance.gameObject.SetActive(false);
                _stack.Push(instance);
            }
        }

        // プールから最初のアクティブなGameObjectを返す
        public PooledObject GetPooledObject()
        {
            // プールが十分に大きくない場合、新しいPooledObjectをインスタンス化する
            if (_stack.Count == 0)
            {
                var newInstance = Instantiate(_objectToPool, transform);
                newInstance.Pool = this;
                return newInstance;
            }

            // それ以外の場合は、単にリストから次のものを取得する
            var nextInstance = _stack.Pop();
            nextInstance.gameObject.SetActive(true);
            return nextInstance;
        }

        public void ReturnToPool(PooledObject pooledObject)
        {
            _stack.Push(pooledObject);
            pooledObject.gameObject.SetActive(false);
        }
    }
}