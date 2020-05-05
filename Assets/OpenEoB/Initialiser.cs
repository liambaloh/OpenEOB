using System;
using System.Collections.Generic;
using System.IO;
using OpenEoB.Views;
using UnityEngine;

namespace OpenEoB
{
    public class Initialiser : MonoBehaviour
    {
        [SerializeField] private TileView _tilePrefab;
        [SerializeField] private Transform _tilesParent;

        void Start()
        {
            Initialise();
        }

        private void Initialise()
        {
            GenerateMap();
        }

        private void GenerateMap()
        {
            var mapLines = File.ReadAllLines(Application.streamingAssetsPath + "/map.txt");

            for (var lineNumber = 0; lineNumber < mapLines.Length; lineNumber++)
            {
                var line = mapLines[lineNumber].Trim();
                var tileStrings = line.Split(';');
                for (var tileIndex = 0; tileIndex < tileStrings.Length; tileIndex++)
                {
                    var tileString = tileStrings[tileIndex];
                    var tileX = lineNumber;
                    var tileY = tileIndex;

                    if (tileString.StartsWith("[") && tileString.EndsWith("]"))
                    {
                        tileString = tileString.Substring(1, tileString.Length - 2);
                        var tileDescriptors = tileString.Split('|');
                        if (tileDescriptors.Length != 6)
                        {
                            throw new Exception("Cannot parse tile descriptor: "+tileDescriptors);
                        }
                        
                        var wallNorthId = tileDescriptors[0].Trim();
                        var wallSouthId = tileDescriptors[1].Trim();
                        var wallEastId = tileDescriptors[2].Trim();
                        var wallWestId = tileDescriptors[3].Trim();
                        var floorId = tileDescriptors[4].Trim();
                        var ceilingId = tileDescriptors[5].Trim();

                        var tile = Instantiate(_tilePrefab, _tilesParent);
                        var tileTransform = tile.transform;

                        tileTransform.localPosition = new Vector3(tileY, 0, tileX);
                        tileTransform.localScale = Vector3.one;
                        tileTransform.localRotation = Quaternion.identity;

                        tile.Setup(tileX, tileY, wallNorthId, wallSouthId, wallEastId, wallWestId, floorId, ceilingId);
                    }
                }
            }
        }
    }
}
