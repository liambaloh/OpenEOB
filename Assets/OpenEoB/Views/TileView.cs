using System.Collections.Generic;
using OpenEoB.Config;
using UnityEngine;

namespace OpenEoB.Views
{
    public class TileView : MonoBehaviour
    {
        private const string BlankTileObjectId = "XX";
        private const string BlankNpctId = "YA";

        public int X { get; private set; }
        public int Y { get; private set; }
        
        [SerializeField] private TileGraphicsConfig _tileGraphicsConfig;
        [SerializeField] private TileObjectConfig _tileObjectConfig;
        [SerializeField] private NpcConfig _npcConfig;
        [SerializeField] private TileGraphics _wallNorth;
        [SerializeField] private TileGraphics _wallSouth;
        [SerializeField] private TileGraphics _wallEast;
        [SerializeField] private TileGraphics _wallWest;
        [SerializeField] private TileGraphics _floor;
        [SerializeField] private TileGraphics _ceiling;
        [SerializeField] private Transform _objectsParent;
        [SerializeField] private Transform _npcsParent;
        private List<TileObjectView> _tileObjects;
        private List<NpcView> _npcs;

        public void Setup(int x, int y, string wallNorthId, string wallSouthId, string wallEastId, string wallWestId, string floorId, string ceilingId, string tileObjectId, string npcId)
        {
            X = x;
            Y = y;
            _tileObjects = new List<TileObjectView>();
            _npcs = new List<NpcView>();
            
            _wallNorth.SetGraphic(_tileGraphicsConfig, wallNorthId);
            _wallSouth.SetGraphic(_tileGraphicsConfig, wallSouthId);
            _wallEast.SetGraphic(_tileGraphicsConfig, wallEastId);
            _wallWest.SetGraphic(_tileGraphicsConfig, wallWestId);
            _floor.SetGraphic(_tileGraphicsConfig, floorId);
            _ceiling.SetGraphic(_tileGraphicsConfig, ceilingId);

            if (tileObjectId != BlankTileObjectId)
            {
                var tileObjectPrefab = _tileObjectConfig.GetTileObjectPrefab(tileObjectId);
                var tileObject = Instantiate(tileObjectPrefab, _objectsParent);
                var tileObjectTransform = tileObject.transform;
                tileObjectTransform.localPosition = Vector3.zero;
                tileObjectTransform.localRotation = Quaternion.identity;
                tileObjectTransform.localScale = Vector3.one;
                
                tileObject.Setup(this);
                _tileObjects.Add(tileObject);
            }

            if (npcId != BlankNpctId)
            {
                var npcDatum = _npcConfig.GetNpcDatum(npcId);
                var npcPrefab = npcDatum.Prefab;
                var npc = Instantiate(npcPrefab, _npcsParent);
                var npcTransform = npc.transform;
                npcTransform.localPosition = Vector3.zero;
                npcTransform.localRotation = Quaternion.identity;
                npcTransform.localScale = Vector3.one;
                
                npc.Setup(npcDatum);
                _npcs.Add(npc);
            }
        }

        public bool CanPlayerEnterTile()
        {
            foreach (var tileObjectView in _tileObjects)
            {
                if (tileObjectView.CanPlayerEnterTile())
                {
                    continue;
                }

                return false;
            }

            foreach (var npc in _npcs)
            {
                if (npc.CanPlayerEnterTile())
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        public void Bump()
        {
            foreach (var tileObjectView in _tileObjects)
            {
                tileObjectView.Bump();
            }
            
            foreach (var npc in _npcs)
            {
                npc.Bump();
            }
        }
    }
}
