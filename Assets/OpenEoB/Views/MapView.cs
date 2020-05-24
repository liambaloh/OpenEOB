using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OpenEoB.Views
{
    public class MapView : MonoBehaviour
    {
        [SerializeField] private TileView _tilePrefab;
        [SerializeField] private Transform _tilesParent;

        private Dictionary<Tuple<int, int>, TileView> _tiles;

        private void Awake()
        {
            _tiles = new Dictionary<Tuple<int, int>, TileView>();
        }

        public bool TileExists(int x, int y)
        {
            return _tiles.ContainsKey(new Tuple<int, int>(x, y));
        }

        public TileView GetTile(int x, int y)
        {
            return _tiles[new Tuple<int, int>(x, y)];
        }

        public void GenerateMap(string mapName)
        {
            var mapLines = File.ReadAllLines(Application.streamingAssetsPath + "/" + mapName + ".txt");

            for (var lineNumber = 0; lineNumber < mapLines.Length; lineNumber++)
            {
                var line = mapLines[lineNumber].Trim();
                var tileStrings = line.Split(';');
                for (var tileIndex = 0; tileIndex < tileStrings.Length; tileIndex++)
                {
                    var tileString = tileStrings[tileIndex];
                    var tileX = tileIndex;
                    var tileY = mapLines.Length - lineNumber;

                    if (tileString.StartsWith("[") && tileString.EndsWith("]"))
                    {
                        tileString = tileString.Substring(1, tileString.Length - 2);
                        var tileDescriptors = tileString.Split('|');
                        if (tileDescriptors.Length != 7)
                        {
                            throw new Exception("Cannot parse tile descriptor: " + tileDescriptors);
                        }

                        var wallNorthId = tileDescriptors[0].Trim();
                        var wallSouthId = tileDescriptors[1].Trim();
                        var wallEastId = tileDescriptors[2].Trim();
                        var wallWestId = tileDescriptors[3].Trim();
                        var floorId = tileDescriptors[4].Trim();
                        var ceilingId = tileDescriptors[5].Trim();
                        var tileObjectId = tileDescriptors[6].Trim();

                        var tile = Instantiate(_tilePrefab, _tilesParent);
                        var tileTransform = tile.transform;

                        tileTransform.localPosition = new Vector3(tileX, 0, tileY);
                        tileTransform.localScale = Vector3.one;
                        tileTransform.localRotation = Quaternion.identity;

                        tile.Setup(tileX, tileY, wallNorthId, wallSouthId, wallEastId, wallWestId, floorId, ceilingId,
                            tileObjectId);

                        _tiles.Add(new Tuple<int, int>(tileX, tileY), tile);
                    }
                }
            }
        }
    }
}