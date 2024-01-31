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
        void Start()
        {
            InputProvider.Instance.SelectHotbarAxis.Subscribe(axis => _hotbar.Scroll(axis, ItemContainer, EquipmentItem)).AddTo(this);
            InputProvider.Instance.UseTrigger.Subscribe(UseItem).AddTo(this);
            InputProvider.Instance.DropTrigger.Subscribe(_=> Drop(_hotbar.SelectIndex)).AddTo(this);
            //  アイテムコンテナが更新されたらスロットも更新する
            ItemContainer.ObserveCountChanged().Subscribe(_=>
            {
                _hotbar.UpdateSlots(ItemContainer);
                EquipmentItem();
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
                _armsAnimator.SetBool("GrabItem", false);
                return;
            }
            if (!EquippedItem)
            {
                Equipment(_hotbar.SelectIndex);
                _armsAnimator.SetBool("GrabItem", true);
            }
            else
            {
                _armsAnimator.SetBool("GrabItem", false);
            }
        }
        
        void UseItem(Unit _)
        {
            if (!EquippedItem) return;
            if (EquippedItem.TryGetComponent(out IUsable usable))
            {
                usable.Use();
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
