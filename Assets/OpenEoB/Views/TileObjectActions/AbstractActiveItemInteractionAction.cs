using OpenEoB.Views.Items;

namespace OpenEoB.Views.TileObjectActions
{
    public abstract class AbstractActiveItemInteractionAction : AbstractTileObjectAction
    {
        public abstract void InteractWithItem(ItemView item);
        
        public override void PerformAction()
        {

        }
        
        protected void ConsumeActiveItem()
        {
            if (PlayerView.Instance.HasActiveItem())
            {
                PlayerView.Instance.RemoveActiveItem();
            }
        }
    }
}