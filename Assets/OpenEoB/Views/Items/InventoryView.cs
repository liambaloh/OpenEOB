using System;
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
        private PlayerView _player;

        private void Awake()
        {
            _items = new Dictionary<int, ItemView>();
            for (int i = 0; i < InventoryCapacity; i++)
            {
                _items.Add(i, null);
            }
        }

        public void Setup(PlayerView player)
        {
            _player = player;

            for (var i = 0; i < _inventorySlotViews.Count; i++)
            {
                _inventorySlotViews[i].Setup(this, i);
            }

            InputManager.Instance.RegisterAction(Command.ScrollInventoryForward, this, ScrollInventoryForward);
            InputManager.Instance.RegisterAction(Command.ScrollInventoryBackward, this, ScrollInventoryBackward);
            
            InputManager.Instance.RegisterAction(Command.SelectItemInSlotView0, this,
                () => { PutOrTakeItemFromSlotView(0); });
            InputManager.Instance.RegisterAction(Command.SelectItemInSlotView1, this,
                () => { PutOrTakeItemFromSlotView(1); });
            InputManager.Instance.RegisterAction(Command.SelectItemInSlotView2, this,
                () => { PutOrTakeItemFromSlotView(2); });

            //todo remove
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

        private void PutOrTakeItemFromSlotView(int slotViewIndex)
        {
            var inventoryItemIndex = _inventorySlotViews[slotViewIndex].InventoryItemIndex;
            if (_player.HasActiveItem())
            {
                var playerActiveItem = _player.GetActiveItem();
                if (IsInventorySlotFilled(inventoryItemIndex))
                {
                    var itemPreviouslyInItemSlot = GetItemInInventorySlot(inventoryItemIndex);
                    _player.SetActiveItem(itemPreviouslyInItemSlot);
                }
                else
                {
                    _player.RemoveActiveItem();
                }

                SetItemInInventorySlot(inventoryItemIndex, playerActiveItem);
            }
            else
            {
                if (IsInventorySlotFilled(inventoryItemIndex))
                {
                    var itemPreviouslyInItemSlot = GetItemInInventorySlot(inventoryItemIndex);
                    _player.SetActiveItem(itemPreviouslyInItemSlot);
                    RemoveItemInInventorySlot(inventoryItemIndex);
                }
            }
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

        public void RemoveItemInInventorySlot(int slotIndex)
        {
            _items[slotIndex] = null;
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