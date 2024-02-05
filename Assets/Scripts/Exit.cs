using System;
using System.Text;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MonstersDomain
{
    public class Exit : MonoBehaviour
    {
        [SerializeField, Tooltip("必要なアイテム")] ItemData _requestItem;
        [SerializeField, Range(1, 100)] int _requestItemAmount = 1;
        [SerializeField] Image _requestItemImage;
        [SerializeField] Text _requestItemText;
        int _currentItemAmount = 0;
        IDisposable _disposable;

        void Awake()
        {
            _requestItemImage.sprite = _requestItem.Icon;
        }

        void Update()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append(_currentItemAmount).Append(" / ").Append(_requestItemAmount);
            _requestItemText.text = stringBuilder.ToString();
        }

        void OnDisable()
        {
            _disposable?.Dispose();
        }

        void RequestItem(Unit _)
        {
            if (_requestItemAmount <= _currentItemAmount) return;
            if (!ItemManager.Instance.RequestItem(_requestItem.Id)) return;
            _currentItemAmount++;
            if (_requestItemAmount <= _currentItemAmount)
            {
                GameManager.Instance.CurrentGameState.Value = GameState.GameClear;
            }
        }
        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                InteractionMessage.Instance.WriteText("[E] アイテムを捧げる");
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
