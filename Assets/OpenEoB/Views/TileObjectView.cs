using OpenEoB.Config;
using UnityEngine;

namespace OpenEoB.Views
{
    public class TileObjectView : MonoBehaviour
    {
        protected TileView Tile;

        protected virtual void Start()
        {
            
        }

        public void Setup(TileView tileView)
        {
            Tile = tileView;
        }
    }
}