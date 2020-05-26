using OpenEoB.Views.TileObjectActions;
using UnityEngine;

namespace OpenEoB.Views.ClickableObjectViews
{
    public class ActiveItemInteractionClickableObject : AbstractClickableObjectView
    {
        [SerializeField] private AbstractActiveItemInteractionAction _activeItemInteractionAction;

        protected override void Clicked()
        {
            if (PlayerView.Instance.HasActiveItem())
            {
                var activeItem = PlayerView.Instance.GetActiveItem();
                _activeItemInteractionAction.InteractWithItem(activeItem);
            }
        }
    }
}
