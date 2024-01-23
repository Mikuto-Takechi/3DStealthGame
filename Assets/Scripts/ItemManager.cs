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
            .Subscribe(axis => 
            {
                _hotbar.Scroll(axis, _items, CheckItem);
            });
        _hotbar.UpdateSlots(_items);
        CheckItem();
    }
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
}
