using System.Collections.Generic;
using OpenEoB.Config;
using OpenEoB.Views.UI;
using UnityEngine;

namespace OpenEoB.Views.Items
{
    public class InventoryView : MonoBehaviour
    {
        public const int InventoryCapacity = 20;

        [SerializeField] private List<InventorySlotView> _inventorySlotViews;
        [SerializeField] private ItemConfig _itemConfig;

        private Dictionary<int, ItemView> _items;

        private void Awake()
        {
            _items = new Dictionary<int, ItemView>();
            for (int i = 0; i < InventoryCapacity; i++)
            {
                _items.Add(i, null);
            }
        }

        private void Start()
        {
            for (var i = 0; i < _inventorySlotViews.Count; i++)
            {
                _inventorySlotViews[i].Setup(this, i);
            }

            InputManager.Instance.RegisterAction(Command.ScrollInventoryForward, this, ScrollInventoryForward);
            InputManager.Instance.RegisterAction(Command.ScrollInventoryBackward, this, ScrollInventoryBackward);
            
            var item1 = new ItemView(_itemConfig.GetItemDatum("KEY"));
            var item2 = new ItemView(_itemConfig.GetItemDatum("SWORD"));
            var item3 = new ItemView(_itemConfig.GetItemDatum("ARMOR"));
            var item4 = new ItemView(_itemConfig.GetItemDatum("BOW"));
            var item5 = new ItemView(_itemConfig.GetItemDatum("LOCKPICK"));
            SetItemInInventorySlot(1, item1);
            SetItemInInventorySlot(2, item2);
            SetItemInInventorySlot(5, item3);
            SetItemInInventorySlot(10, item4);
            SetItemInInventorySlot(18, item5);
        }

        public bool IsInventorySlotFilled(int slotIndex)
        {
            return _items[slotIndex] != null;
        }

        public void SetItemInInventorySlot(int slotIndex, ItemView item)
        {
            //todo swap
            //todo remove from previous slot, if present
            _items[slotIndex] = item;
            UpdateDisplayForInventoryItemSlot(slotIndex);
        }

        public ItemView GetItemInInventorySlot(int slotIndex)
        {
            return _items[slotIndex];
        }

        private void ScrollInventoryForward()
        {
            foreach (var inventorySlotView in _inventorySlotViews)
            {
                inventorySlotView.IncrementInventoryItemIndex(+1);
            }
        }

        private void ScrollInventoryBackward()
        {
            foreach (var inventorySlotView in _inventorySlotViews)
            {
                inventorySlotView.IncrementInventoryItemIndex(-1);
            }
        }

        private void UpdateDisplayForInventoryItemSlot(int inventoryItemSlot)
        {
            for (var i = 0; i < _inventorySlotViews.Count; i++)
            {
                if (_inventorySlotViews[i].InventoryItemIndex == inventoryItemSlot)
                {
                    _inventorySlotViews[i].UpdateCurrentItemGraphic();
                }
            }
        }
    }
}