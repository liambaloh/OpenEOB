using OpenEoB.Events;
using OpenEoB.Views.Items;
using UnityEngine;

namespace OpenEoB.Views.TileObjectActions.ActiveItemInteractionActions
{
    public class TriggerEventActiveItemInteractionAction : AbstractActiveItemInteractionAction
    {
        [SerializeField] private string _itemIdToUnlock;
        [SerializeField] private EventId _eventId;

        public override void InteractWithItem(ItemView item)
        {
            if (item.Id == _itemIdToUnlock)
            {
                ConsumeActiveItem();
                EventManager.Instance.TriggerEvent(_eventId);
            }
        }
    }
}