using UnityEngine;
using UnityEngine.UI;

namespace OpenEoB.Views.Items
{
    public class InventorySlotView : MonoBehaviour
    {
        public int InventoryItemIndex { get; private set; }

        [SerializeField] private Image _itemImage;

        private InventoryView _inventoryView;

        public void Setup(InventoryView inventoryView, int initialInventoryItemIndex)
        {
            _inventoryView = inventoryView;
            SetNewInventoryItemIndex(initialInventoryItemIndex);
        }

        private void SetNewInventoryItemIndex(int newInventoryItemIndex)
        {
            InventoryItemIndex = newInventoryItemIndex;
            UpdateCurrentItemGraphic();
        }

        public void UpdateCurrentItemGraphic()
        {
            if (_inventoryView.IsInventorySlotFilled(InventoryItemIndex))
            {
                var itemView = _inventoryView.GetItemInInventorySlot(InventoryItemIndex);
                _itemImage.sprite = itemView.ItemSprite;
                _itemImage.color = Color.white;
            }
            else
            {
                _itemImage.sprite = null;
                _itemImage.color = new Color(0, 0, 0, 0);
            }
        }

        public void IncrementInventoryItemIndex(int difference)
        {
            InventoryItemIndex += difference;
            InventoryItemIndex %= InventoryView.InventoryCapacity;
            if (InventoryItemIndex < 0)
            {
                InventoryItemIndex += InventoryView.InventoryCapacity;
            }
            UpdateCurrentItemGraphic();
        }
    }
}