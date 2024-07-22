using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace MonstersDomain
{
    public class Door : Interactable
    {
        [SerializeField] float _animationTime = 0.4f;
        bool _isOpened;
        IDisposable _enterSubscription;
        Animator _animator;
        CancellationTokenSource _cts;
        bool _isInteract;
        void Awake()
        {
            _cts = new();
            _animator = GetComponent<Animator>();
        }

        protected override void Interact(Player player)
        {
            if (_isInteract) return;
            _isInteract = true;
            SetText();
            _enterSubscription = InputProvider.Instance.InteractTrigger.First().Subscribe(_ =>
            {
                SwitchDoor(_cts.Token).Forget();
            }).AddTo(this);
        }

        protected override void Disengage(Player player)
        {
            _isInteract = false;
            InteractionMessage.Instance.WriteText(string.Empty);
            _enterSubscription?.Dispose();
            _cts?.Cancel();
            _cts = new();
        }

        async UniTaskVoid SwitchDoor(CancellationToken ct)
        {
            _isOpened = !_isOpened;
            _animator.SetBool("IsOpened", _isOpened);
            AudioManager.Instance.Play3DSE(_isOpened ? SE.OpenDoor : SE.CloseDoor, transform.position);
            await UniTask.WaitForSeconds(_animationTime, cancellationToken: ct);
            _isInteract = false;
            Interact(null);
        }

        void SetText()
        {
            if (_isOpened)
            {
                InteractionMessage.Instance.WriteText("[E] 閉じる");
            }
            else
            {
                InteractionMessage.Instance.WriteText("[E] 開く");
            }
        }
    }
}