using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MonstersDomain
{
    public class Exit : MonoBehaviour
    {
        [SerializeField, Tooltip("必要なアイテム")] ItemData _requestItem;
        [SerializeField, Range(1, 100)] int _requestItemAmount = 1;
        int _currentItemAmount = 0;
        IDisposable _disposable;

        void OnDisable()
        {
            _disposable?.Dispose();
        }

        void RequestItem(Unit _)
        {
            if (_requestItemAmount <= _currentItemAmount) return;
            if (!ItemManager.Instance.RequestItem(_requestItem.Id)) return;
            _currentItemAmount++;
            InteractionMessage.Instance.WriteText($"[E] 脱出する。 \n 要求アイテム {_requestItem.DisplayName} {_currentItemAmount} / {_requestItemAmount}");
            if (_requestItemAmount <= _currentItemAmount)
            {
                GameManager.Instance.CurrentGameState.Value = GameState.GameClear;
            }
        }
        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                InteractionMessage.Instance.WriteText($"[E] 脱出する。 \n 要求アイテム {_requestItem.DisplayName} {_currentItemAmount} / {_requestItemAmount}");
                _disposable?.Dispose();
                _disposable = InputProvider.Instance.InteractTrigger.Subscribe(RequestItem).AddTo(this);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                InteractionMessage.Instance.WriteText("");
                _disposable?.Dispose();
            }
        }
    }
}
