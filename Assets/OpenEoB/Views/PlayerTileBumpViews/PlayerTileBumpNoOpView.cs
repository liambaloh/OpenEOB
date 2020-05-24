namespace OpenEoB.Views.PlayerTileBumpViews
{
    public class PlayerTileBumpNoOpView : AbstractPlayerTileBumpView
    {
        public override void Bump()
        {
            UnityEngine.Debug.Log("Bump!");
        }
    }
}