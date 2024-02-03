using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonstersDomain
{
    [Serializable]
    public class SerializedHashSet<T> : HashSet<T>, ISerializationCallbackReceiver
    {
        [SerializeField] List<T> _list;
        public virtual T DefaultValue => default;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();
            foreach (var item in _list) Add(Contains(item) ? DefaultValue : item);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _list = new List<T>(Count);
            foreach (var item in this) _list.Add(item);
        }
    }
    [Serializable]
    public class SerializedHashSetClass<T> : SerializedHashSet<T> where T : new()
    {
        public override T DefaultValue => new();
    }
}