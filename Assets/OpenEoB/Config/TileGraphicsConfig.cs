using System;
using System.Collections.Generic;
using UnityEngine;

namespace OpenEoB.Config
{
    [CreateAssetMenu(fileName = "TileGraphicsConfig", menuName = "Config/TileGraphicsConfig", order = 1)]
    public class TileGraphicsConfig : ScriptableObject
    {
        [SerializeField]private List<TileGraphicsDatum> _tileGraphics;

        public Texture GetTexture(string id)
        {
            foreach (var tileGraphicsDatum in _tileGraphics)
            {
                if (tileGraphicsDatum.Id == id)
                {
                    return tileGraphicsDatum.Texture;
                }
            }

            throw new Exception("Could not find tile graphic with id "+id);
        }
    }
}
