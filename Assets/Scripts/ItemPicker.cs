using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace MonstersDomain
{
    public class ItemPicker : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f), Tooltip("アイテムとカメラの内積で選択可能になる閾値。1に近ければより照準があっている。")] float _itemDotThreshold = 0.97f;
        Camera _main;
        Vector3ReactiveProperty _updateAngle = new();
        List<DroppedItem> _pickableList = new();
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
                    if (pickable != null && Vector3.Dot(_main.transform.forward,
                            (pickable.transform.position - _main.transform.position).normalized) >
                        _itemDotThreshold)
                    {
                        InteractionMessage.Instance.WriteText("[E] 拾う");
                        CheckIfSubscribed(pickable);
                        foundPickable = true;
                        break;
                    }
                }

                _pickableList.RemoveAll(p => p == null);

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
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out DroppedItem pickable))
            {
                //  リストに追加
                _pickableList.Add(pickable);
            }
        }
        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out DroppedItem pickable))
            {
                _pickableList = _pickableList.Where(p => p.GetHashCode() != pickable.GetHashCode()).ToList();
            }
        }

        void CheckIfSubscribed(DroppedItem droppedItem)
        {
            if (_disposable == null)
            {
                _disposable = InputProvider.Instance.InteractTrigger.Subscribe(_=>droppedItem.PickUp()).AddTo(droppedItem);
            }
            else
            {
                _disposable?.Dispose();
                _disposable = InputProvider.Instance.InteractTrigger.Subscribe(_=>droppedItem.PickUp()).AddTo(droppedItem);
            }
        }
    }
}
