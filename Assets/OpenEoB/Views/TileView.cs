using OpenEoB.Config;
using UnityEngine;

namespace OpenEoB.Views
{
    public class TileView : MonoBehaviour
    {
        private const string BlankTileObjectId = "XX";

        public int X { get; private set; }
        public int Y { get; private set; }
        
        [SerializeField] private TileGraphicsConfig _tileGraphicsConfig;
        [SerializeField] private TileObjectConfig _tileObjectConfig;
        [SerializeField] private TileGraphics _wallNorth;
        [SerializeField] private TileGraphics _wallSouth;
        [SerializeField] private TileGraphics _wallEast;
        [SerializeField] private TileGraphics _wallWest;
        [SerializeField] private TileGraphics _floor;
        [SerializeField] private TileGraphics _ceiling;
        [SerializeField] private Transform _objectsParent;

        public void Setup(int x, int y, string wallNorthId, string wallSouthId, string wallEastId, string wallWestId, string floorId, string ceilingId, string tileObjectId)
        {
            X = x;
            Y = y;
            
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
            }
        }
    }
}
