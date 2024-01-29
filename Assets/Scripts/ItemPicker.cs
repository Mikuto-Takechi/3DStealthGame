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
        List<IPickable> _pickableList = new();
        IPickable _canPickUp;
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
                            (((Component)pickable).transform.position - _main.transform.position).normalized) >
                        _itemDotThreshold)
                    {
                        InteractionMessage.Instance.WriteText("[E] 拾う");
                        _canPickUp = pickable;
                        foundPickable = true;
                        break;
                    }
                }

                if (!foundPickable)
                {
                    InteractionMessage.Instance.WriteText("");
                    _canPickUp = null;
                }
            }
        }
        void Update()
        {
            _updateAngle.Value = _main.transform.localEulerAngles;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IPickable pickable))
            {
                _pickableList.Add(pickable);
            }
        }
        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IPickable pickable))
            {
                _pickableList = _pickableList.Where(p => p.GetHashCode() != pickable.GetHashCode()).ToList();
            }
        }
    }
}
