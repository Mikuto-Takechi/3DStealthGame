using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MonstersDomain.Common;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MonstersDomain
{
    public class Hotbar : MonoBehaviour
    {
        [SerializeField] ItemDataBase _itemDataBase;

        [SerializeField, Tooltip("実際に生成するスロットの個数")]
        int _slotCount = 5;

        [SerializeField] RectTransform _root;
        [SerializeField] ItemSlot _slotPrefab;
        [SerializeField] int _selectIndex;
        [SerializeField] Vector3 _selectScale = new(1.2f, 1.2f, 1.2f);
        Vector2 _initialPosition = Vector2.zero;
        Sequence _sequence;
        readonly List<ItemSlot> _slotContainer = new();

        public int SelectIndex
        {
            get => _selectIndex;
            set => _selectIndex = value;
        }

        void Awake()
        {
            _initialPosition = _root.anchoredPosition;
            //  スロット生成
            for (var i = 0; i < _slotCount; i++)
            {
                var slot = Instantiate(_slotPrefab, _root, false);
                slot.transform.SetAsLastSibling();
                _slotContainer.Add(slot);
            }

            //  中央のスロットのスケールを上げて強調表示する
            _slotContainer[_slotCount / 2].transform.localScale = _selectScale;
        }

        public void UpdateSlots(ReactiveCollection<(ItemId, List<ItemParameter>)> itemContainer)
        {
            if (itemContainer.Count <= 0)
            {
                _slotContainer.ForEach(s => s.NotContains());
                return;
            }

            var setIndex = ArrayUtil.CircularBuffer(_selectIndex, itemContainer.Count);

            for (var i = _slotCount / 2; i < _slotCount; i++)
            {
                //  選択しているスロットから後ろを埋める
                _slotContainer[i].Set(_itemDataBase[itemContainer[setIndex].Item1], itemContainer.Count - 1 == setIndex,
                    0 == setIndex);
                setIndex = ArrayUtil.CircularBuffer(setIndex + 1, itemContainer.Count);
            }

            setIndex = ArrayUtil.CircularBuffer(_selectIndex - 1, itemContainer.Count);

            for (var i = _slotCount / 2 - 1; i >= 0; i--)
            {
                //  選択しているスロットの前を埋める
                _slotContainer[i].Set(_itemDataBase[itemContainer[setIndex].Item1], itemContainer.Count - 1 == setIndex,
                    0 == setIndex);
                setIndex = ArrayUtil.CircularBuffer(setIndex - 1, itemContainer.Count());
            }
        }

        public void Scroll(float input, ReactiveCollection<(ItemId, List<ItemParameter>)> itemContainer,
            Action callback)
        {
            if (_sequence != null || itemContainer.Count <= 0) return;
            AudioManager.Instance.PlaySE(SE.SelectHotbar);
            if (input > 0)
            {
                _selectIndex = ArrayUtil.CircularBuffer(_selectIndex + 1, itemContainer.Count);
                callback?.Invoke();
                var spacing = _root.GetComponent<HorizontalLayoutGroup>().spacing;
                _sequence = DOTween.Sequence().OnComplete(() =>
                {
                    _sequence = null;
                    UpdateSlots(itemContainer);
                    _root.anchoredPosition = _initialPosition;
                    _slotContainer[_slotCount / 2].transform.localScale = _selectScale;
                    _slotContainer[_slotCount / 2 + 1].transform.localScale = Vector3.one;
                }).SetLink(gameObject);
                _sequence.Join(_root.DOAnchorPosX(_root.anchoredPosition.x - 100 - spacing, 0.5f));
                _sequence.Join(_slotContainer[_slotCount / 2].transform.DOScale(1f, 0.5f));
                _sequence.Join(_slotContainer[_slotCount / 2 + 1].transform.DOScale(_selectScale[0], 0.5f));
            }
            else if (input < 0)
            {
                _selectIndex = ArrayUtil.CircularBuffer(_selectIndex - 1, itemContainer.Count);
                callback?.Invoke();
                var spacing = _root.GetComponent<HorizontalLayoutGroup>().spacing;
                _sequence = DOTween.Sequence().OnComplete(() =>
                {
                    _sequence = null;
                    UpdateSlots(itemContainer);
                    _root.anchoredPosition = _initialPosition;
                    _slotContainer[_slotCount / 2].transform.localScale = _selectScale;
                    _slotContainer[_slotCount / 2 - 1].transform.localScale = Vector3.one;
                }).SetLink(gameObject);
                _sequence.Join(_root.DOAnchorPosX(_root.anchoredPosition.x + 100 + spacing, 0.5f));
                _sequence.Join(_slotContainer[_slotCount / 2].transform.DOScale(1f, 0.5f));
                _sequence.Join(_slotContainer[_slotCount / 2 - 1].transform.DOScale(_selectScale[0], 0.5f));
            }
        }
    }
}