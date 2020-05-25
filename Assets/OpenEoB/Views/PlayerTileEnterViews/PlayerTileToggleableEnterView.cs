using UnityEngine;

namespace OpenEoB.Views.PlayerTileEnterViews
{
    public class PlayerTileToggleableEnterView : AbstractPlayerTileEnterView
    {
        [SerializeField] private bool _canPlayerEnterTile;

        public override bool CanPlayerEnterTile()
        {
            return _canPlayerEnterTile;
        }

        public void SetCanPlayerEnterView(bool canPlayerEnterTile)
        {
            _canPlayerEnterTile = canPlayerEnterTile;
        }
    }
}