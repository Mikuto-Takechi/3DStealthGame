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
            if (ItemContainer.Count <= 0) return;
            if (!EquippedItem)
            {
                Equipment(_hotbar.SelectIndex);
                _armsAnimator.SetBool("GrabItem", true);
            }
            else
            {
                UnEquipment();
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
            EquipmentItem();
            _hotbar.UpdateSlots(ItemContainer);
        }
    }
}
