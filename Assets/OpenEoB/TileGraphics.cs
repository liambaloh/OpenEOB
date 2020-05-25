using OpenEoB.Config;
using UnityEngine;

namespace OpenEoB
{
    [System.Serializable]
    public class TileGraphics
    {
        public const string BlankWallId = "EE";
        public Renderer TileRenderer;
        public Collider TileCollider;
        
        public void SetGraphic(TileGraphicsConfig tileGraphicsConfig, string graphicsId)
        {
            if (graphicsId == BlankWallId)
            {
                TileRenderer.enabled = false;
                TileCollider.enabled = false;
            }
            else
            {
                TileRenderer.enabled = true;
                TileCollider.enabled = true;
                TileRenderer.material = new Material(TileRenderer.material);
                TileRenderer.material.mainTexture = tileGraphicsConfig.GetTexture(graphicsId);
            }
        }
    }
}
