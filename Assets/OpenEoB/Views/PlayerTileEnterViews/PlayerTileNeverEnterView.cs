namespace OpenEoB.Views.PlayerTileEnterViews
{
    public class PlayerTileNeverEnterView : AbstractPlayerTileEnterView
    {
        public override bool CanPlayerEnterTile()
        {
            return false;
        }
    }
}