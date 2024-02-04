using System;
using UniRx;
using UnityEngine;

namespace MonstersDomain
{
    public class ItemManager : InventorySystem
    {
        public static ItemManager Instance;
        [SerializeField] Hotbar _hotbar;
        [SerializeField] Animator _armsAnimator;
        void Awake()
        {
            Instance ??= this;
        }

        void OnDisable()
        {
            Instance = null;
        }

        void Start()
        {
            InputProvider.Instance.SelectHotbarAxis.Subscribe(axis => _hotbar.Scroll(axis, ItemContainer, EquipmentItem)).AddTo(this);
            InputProvider.Instance.UseTrigger.Subscribe(UseItem).AddTo(this);
            InputProvider.Instance.DropTrigger.Subscribe(_=> Drop(_hotbar.SelectIndex)).AddTo(this);
            //  アイテムコンテナが更新されたらスロットも更新する
            ItemContainer.ObserveRemove().Subscribe(_=>
            {
                _hotbar.UpdateSlots(ItemContainer);
                EquipmentItem();
            }).AddTo(this);
            ItemContainer.ObserveAdd().Subscribe(_=>
            {
                _hotbar.UpdateSlots(ItemContainer);
                if (ItemContainer.Count <= 1) EquipmentItem();
            }).AddTo(this);
            EquipmentItem();
        }
        /// <summary>
        /// ホットバーで選択しているアイテムを見て手にセットする
        /// </summary>
        void EquipmentItem()
        {
            UnEquipment();
            if (ItemContainer.Count <= 0)
            {
                _armsAnimator.SetInteger("HoldItem", 0);
                return;
            }
            if (!EquippedItem)
            {
                Equipment(_hotbar.SelectIndex);
                if(EquippedItem) _armsAnimator.SetInteger("HoldItem", EquippedItem.HoldType);
            }
            else
            {
                _armsAnimator.SetInteger("HoldItem", 0);
            }
        }
        
        void UseItem(Unit _)
        {
            if (!EquippedItem) return;
            if (EquippedItem.TryGetComponent(out IUsable usable))
            {
                usable.Use();
            }

            if (EquippedItem.TryGetComponent(out IConsumable consumable))
            {
                if (consumable.Consume(ItemContainer))
                {
                    ItemContainer.RemoveAt(_hotbar.SelectIndex);
                    if (_hotbar.SelectIndex >= ItemContainer.Count && ItemContainer.Count > 0)
                    {
                        _hotbar.SelectIndex = ItemContainer.Count - 1;
                    }
                    EquipmentItem();
                    _hotbar.UpdateSlots(ItemContainer);
                }
            }
        }

        protected override void OnDrop()
        {
            if (_hotbar.SelectIndex >= ItemContainer.Count && ItemContainer.Count > 0)
            {
                _hotbar.SelectIndex = ItemContainer.Count - 1;
            }
            
            AudioManager.Instance.PlaySE(SE.Drop);
            EquipmentItem();
            _hotbar.UpdateSlots(ItemContainer);
        }
    }
}
