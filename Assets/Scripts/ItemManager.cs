using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] Item[] _items;
    [SerializeField] Hotbar _hotbar;
    [SerializeField] Transform _itemAnchor;
    [SerializeField] Animator _armsAnimator;
    GameObject _grabItem;
    void Start()
    {
        this.UpdateAsObservable()
            .Select(_ => Input.GetAxisRaw("Mouse ScrollWheel"))
            .Subscribe(axis => _hotbar.Scroll(axis, _items, CheckItem)).AddTo(this);
        this.UpdateAsObservable()
            .Where(_ => Input.GetButtonDown("Use"))
            .Subscribe(UseItem).AddTo(this);
        _hotbar.UpdateSlots(_items);
        CheckItem();
    }
    /// <summary>
    /// ホットバーで選択しているアイテムを見て手にセットする
    /// </summary>
    void CheckItem()
    {
        var item = _items[_hotbar.SelectIndex].Object;
        Destroy(_grabItem);
        if (item != null)
        {
            _grabItem = Instantiate(item, _itemAnchor);
            _armsAnimator.SetBool("GrabItem", true);
        }
        else
        {
            _armsAnimator.SetBool("GrabItem", false);
        }
    }

    void UseItem(Unit _)
    {
        if (!_grabItem) return;
        if (_grabItem.TryGetComponent(out ITool tool))
        {
            tool.Use();
        }
    }
}
