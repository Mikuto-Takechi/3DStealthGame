using DG.Tweening;
using System;
using System.Collections.Generic;
using Takechi.Algorithm;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    [SerializeField] int _slotCount = 5;
    [SerializeField] RectTransform _root;
    [SerializeField] GameObject _slotPrefab;
    [SerializeField] int _selectIndex = 0;
    [SerializeField] Vector3 _selectScale = new Vector3(1.2f, 1.2f, 1.2f);
    List<GameObject> _slotList = new();
    Sequence _sequence;
    Vector2 _initialPosition = Vector2.zero;
    public int SelectIndex => _selectIndex;

    void Awake()
    {
        _initialPosition = _root.anchoredPosition;
        for (int i = 0; i < _slotCount; i++)
        {
            GameObject slot = Instantiate(_slotPrefab, _root, false);
            slot.transform.SetAsLastSibling();
            _slotList.Add(slot);
        }
        _slotList[_slotCount / 2].transform.localScale = _selectScale;
    }
    public void UpdateSlots(Item[] items)
    {
        int setIndex = Algorithm.CircularBuffer(_selectIndex, items.Length);

        for (int i = _slotCount / 2; i < _slotCount; i++)
        {   //  選択しているスロットから後ろを埋める
            _slotList[i].GetComponentInChildren<Text>().text = items[setIndex].Name;
            setIndex = Algorithm.CircularBuffer(setIndex + 1, items.Length);
        }

        setIndex = Algorithm.CircularBuffer(_selectIndex - 1, items.Length);

        for (int i = _slotCount / 2 - 1; i >= 0; i--)
        {   //  選択しているスロットの前を埋める
            _slotList[i].GetComponentInChildren<Text>().text = items[setIndex].Name;
            setIndex = Algorithm.CircularBuffer(setIndex - 1, items.Length);
        }
    }
    public void Scroll(float input, Item[] items, Action callback)
    {
        if (_sequence != null) return;
        if (input > 0)
        {
            _selectIndex = Algorithm.CircularBuffer(_selectIndex + 1, items.Length);
            float spacing = _root.GetComponent<HorizontalLayoutGroup>().spacing;
            _sequence = DOTween.Sequence().OnComplete(()=>
            {                    
                _sequence = null;
                UpdateSlots(items);
                _root.anchoredPosition = _initialPosition;
                _slotList[_slotCount / 2].transform.localScale = _selectScale;
                _slotList[_slotCount / 2 + 1].transform.localScale = Vector3.one;
                callback();
            }).SetLink(gameObject);
            _sequence.Join(_root.DOAnchorPosX(_root.anchoredPosition.x - 100 - spacing, 0.5f));
            _sequence.Join(_slotList[_slotCount / 2].transform.DOScale(1f, 0.5f));
            _sequence.Join(_slotList[_slotCount / 2 + 1].transform.DOScale(_selectScale[0], 0.5f));
        }
        else if(input < 0)
        {
            _selectIndex = Algorithm.CircularBuffer(_selectIndex - 1, items.Length);
            float spacing = _root.GetComponent<HorizontalLayoutGroup>().spacing;
            _sequence = DOTween.Sequence().OnComplete(()=>
            {                    
                _sequence = null;
                UpdateSlots(items);
                _root.anchoredPosition = _initialPosition;
                _slotList[_slotCount / 2].transform.localScale = _selectScale;
                _slotList[_slotCount / 2 - 1].transform.localScale = Vector3.one;
                callback();
            }).SetLink(gameObject);
            _sequence.Join(_root.DOAnchorPosX(_root.anchoredPosition.x + 100 + spacing, 0.5f));
            _sequence.Join(_slotList[_slotCount / 2].transform.DOScale(1f, 0.5f));
            _sequence.Join(_slotList[_slotCount / 2 - 1].transform.DOScale(_selectScale[0], 0.5f));
        }
    }
}

namespace Takechi.Algorithm
{
    public static class Algorithm
    {
        public static int CircularBuffer(int num, int length)
        {
            if (num < 0)
            {
                return length - 1;
            }
            else
            {
                return num % length;
            }
        }
    }
}