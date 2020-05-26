using OpenEoB.Views.TileObjectActions;
using UnityEngine;

namespace OpenEoB.Views.ClickableObjectViews
{
    public class DoorOpenClickableObject : AbstractClickableObjectView
    {
        [SerializeField] private DoorOpenTileObjectAction _doorOpenTileObjectAction;

        protected override void Clicked()
        {
            _doorOpenTileObjectAction.PerformAction();
        }
    }
}
