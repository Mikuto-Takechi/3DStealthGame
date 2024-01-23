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
    List<GameObject> _slotList = new();
    Tween _tween;
    Vector2 _initialPosition = Vector2.zero;
    public int SelectIndex { get { return _selectIndex; } }
    void Awake()
    {
        _initialPosition = _root.anchoredPosition;
        for (int i = 0; i < _slotCount; i++)
        {
            GameObject slot = Instantiate(_slotPrefab);
            slot.transform.SetParent(_root, false);
            slot.transform.SetAsLastSibling();
            _slotList.Add(slot);
        }
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
        if (_tween != null) return;
        if (input > 0)
        {
            _selectIndex = Algorithm.CircularBuffer(_selectIndex + 1, items.Length);
            float spacing = _root.GetComponent<HorizontalLayoutGroup>().spacing;
            _tween = _root.DOAnchorPosX(_root.anchoredPosition.x - 100 - spacing, 0.5f)
                .OnComplete(()=> 
                { 
                    _tween = null;
                    UpdateSlots(items);
                    _root.anchoredPosition = _initialPosition;
                    callback();
                }).SetLink(gameObject);
        }
        else if(input < 0)
        {
            _selectIndex--;
            _selectIndex = Algorithm.CircularBuffer(_selectIndex, items.Length);
            float spacing = _root.GetComponent<HorizontalLayoutGroup>().spacing;
            _tween = _root.DOAnchorPosX(_root.anchoredPosition.x + 100 + spacing, 0.5f)
                .OnComplete(() =>
                {
                    _tween = null;
                    UpdateSlots(items);
                    _root.anchoredPosition = _initialPosition;
                    callback();
                }).SetLink(gameObject);
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