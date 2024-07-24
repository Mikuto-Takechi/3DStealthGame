using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace MonstersDomain
{
    /// <summary>
    /// アイテムを拾うためのクラス。
    /// </summary>
    public class ItemPicker : MonoBehaviour
    {
        [SerializeField, Tooltip("一人称視点時に表示される腕のアニメーター")] Animator _armsAnimator;
        [SerializeField, Range(0f, 1f), Tooltip("アイテムとカメラの内積で選択可能になる閾値。1に近ければより照準があっている。")]
        float _itemDotThreshold = 0.97f;
        [SerializeField, Tooltip("アイテムを拾うアニメーションの時間")] float _pickUpAnimationTime = 0.4f;
        [SerializeField, Tooltip("アニメーション中に隠すレイヤー")] LayerMask _hideLayerMask = 1 << 11;
        IDisposable _disposable;
        Camera _main;
        List<DroppedItem> _pickableList = new();
        ObservableStateMachineTrigger _trigger;
        readonly Vector3ReactiveProperty _updateAngle = new();
        CancellationTokenSource _cts;

        void Start()
        {
            _cts = new();
            _trigger = _armsAnimator.GetBehaviour<ObservableStateMachineTrigger>();
            _main = Camera.main;
            _updateAngle.SkipLatestValueOnSubscribe().Subscribe(_ => CheckItem()).AddTo(this);
        }

        void Update()
        {
            _updateAngle.Value = _main.transform.localEulerAngles;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out DroppedItem pickable))
                //  リストに追加
                _pickableList.Add(pickable);
        }

        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out DroppedItem pickable))
                _pickableList = _pickableList.Where(p => p.GetHashCode() != pickable.GetHashCode()).ToList();
        }

        void CheckItem()
        {
            if (_pickableList.Count > 0)
            {
                var foundPickable = false;
                foreach (var pickable in _pickableList)
                    if (pickable != null && Vector3.Dot(_main.transform.forward,
                            (pickable.transform.position - _main.transform.position).normalized) >
                        _itemDotThreshold)
                    {
                        InteractionMessage.Instance.WriteText("[E] 拾う");
                        CheckIfSubscribed(pickable);
                        foundPickable = true;
                        break;
                    }

                _pickableList.RemoveAll(p => p == null);

                if (!foundPickable)
                {
                    InteractionMessage.Instance.WriteText("");
                    _disposable?.Dispose();
                }
            }
        }

        /// <summary>
        /// 入力イベント登録処理
        /// </summary>
        void CheckIfSubscribed(DroppedItem droppedItem)
        {
            _disposable?.Dispose();
            _disposable = InputProvider.Instance.InteractTrigger.Subscribe(_ =>
            { 
                _cts.Cancel();
                _cts = new CancellationTokenSource();
                ItemPickUp(droppedItem, _cts.Token).Forget();
            }).AddTo(droppedItem);
        }

        async UniTask ItemPickUp(DroppedItem droppedItem, CancellationToken ct)
        {
            Camera.main.cullingMask &= ~_hideLayerMask;
            droppedItem.PickUp();
            _armsAnimator.SetTrigger("Pickup");
            await UniTask.WaitForSeconds(_pickUpAnimationTime, cancellationToken: ct);
            Camera.main.cullingMask |= _hideLayerMask;
        }
    }
}