using OpenEoB.Config;
using UnityEngine;

namespace OpenEoB
{
    [System.Serializable]
    public class TileGraphics
    {
        public const string BlankWallId = "E";
        public Renderer TileRenderer;
        
        public void SetGraphic(TileGraphicsConfig tileGraphicsConfig, string graphicsId)
        {
            if (graphicsId == BlankWallId)
            {
                TileRenderer.enabled = false;
            }
            else
            {
                TileRenderer.material = new Material(TileRenderer.material);
                TileRenderer.material.mainTexture = tileGraphicsConfig.GetTexture(graphicsId);
            }
        }
    }
}
