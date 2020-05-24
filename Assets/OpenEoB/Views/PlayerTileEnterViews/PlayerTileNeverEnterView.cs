namespace OpenEoB.Views.PlayerTileEnterView
{
    public class PlayerTileNeverEnterView : AbstractPlayerTileEnterView
    {
        public override bool CanPlayerEnterTile()
        {
            return true;
        }
    }
}