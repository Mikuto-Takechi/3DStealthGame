using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace MonstersDomain
{
    public class ItemPicker : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f), Tooltip("アイテムとカメラの内積で選択可能になる閾値。1に近ければより照準があっている。")] float _itemDotThreshold = 0.97f;
        Camera _main;
        Vector3ReactiveProperty _updateAngle = new();
        List<Pickable> _pickableList = new();
        IDisposable _disposable;
        void Start()
        {
            _main = Camera.main;
            _updateAngle.SkipLatestValueOnSubscribe().Subscribe(_=>CheckItem()).AddTo(this);
        }
        void CheckItem()
        {
            if (_pickableList.Count > 0)
            {
                bool foundPickable = false;
                foreach (var pickable in _pickableList)
                {
                    if (Vector3.Dot(_main.transform.forward,
                            (pickable.transform.position - _main.transform.position).normalized) >
                        _itemDotThreshold)
                    {
                        InteractionMessage.Instance.WriteText("[E] 拾う");
                        CheckIfSubscribed(pickable);
                        foundPickable = true;
                        break;
                    }
                }

                if (!foundPickable)
                {
                    InteractionMessage.Instance.WriteText("");
                    _disposable?.Dispose();
                }
            }
        }
        void Update()
        {
            _updateAngle.Value = _main.transform.localEulerAngles;
            print(_pickableList.Count);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Pickable pickable))
            {
                //  リストに追加
                _pickableList.Add(pickable);
                //  追加時のインデックス番号を控えておいてDestroyが呼ばれた際はリストから削除する
                int index = _pickableList.Count - 1;
                pickable.OnDestroyAsObservable().Subscribe(_ =>
                {
                    if (_pickableList.Count > 0)
                        _pickableList.RemoveAt(index);
                });
            }
        }
        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Pickable pickable))
            {
                _pickableList = _pickableList.Where(p => p.GetHashCode() != pickable.GetHashCode()).ToList();
            }
        }

        void CheckIfSubscribed(Pickable pickable)
        {
            if (_disposable == null)
            {
                _disposable = InputProvider.Instance.InteractTrigger.Subscribe(_=>pickable.PickUp()).AddTo(pickable);
            }
            else
            {
                _disposable?.Dispose();
                _disposable = InputProvider.Instance.InteractTrigger.Subscribe(_=>pickable.PickUp()).AddTo(pickable);
            }
        }
    }
}
