using System;
using System.Collections.Generic;
using OpenEoB.Views;
using UnityEngine;

namespace OpenEoB.Config
{
    [CreateAssetMenu(fileName = "TileObjectConfig", menuName = "Config/TileObjectConfig", order = 1)]
    public class TileObjectConfig : ScriptableObject
    {
        [SerializeField] private List<TileObjectDatum> _tileObjects;

        public TileObjectView GetTileObjectPrefab(string id)
        {
            foreach (var tileObjectDatum in _tileObjects)
            {
                if (tileObjectDatum.Id == id)
                {
                    return tileObjectDatum.Prefab;
                }
            }

            throw new Exception("Could not find tile object with id " + id);
        }
    }
}