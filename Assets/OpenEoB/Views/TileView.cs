using OpenEoB.Config;
using UnityEngine;

namespace OpenEoB.Views
{
    public class TileView : MonoBehaviour
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        [SerializeField] private TileGraphicsConfig _tileGraphicsConfig;
        [SerializeField] private TileGraphics _wallNorth;
        [SerializeField] private TileGraphics _wallSouth;
        [SerializeField] private TileGraphics _wallEast;
        [SerializeField] private TileGraphics _wallWest;
        [SerializeField] private TileGraphics _floor;
        [SerializeField] private TileGraphics _ceiling;

        public void Setup(int x, int y, string wallNorth, string wallSouth, string wallEast, string wallWest, string floor, string ceiling)
        {
            X = x;
            Y = y;
            
            _wallNorth.SetGraphic(_tileGraphicsConfig, wallNorth);
            _wallSouth.SetGraphic(_tileGraphicsConfig, wallSouth);
            _wallEast.SetGraphic(_tileGraphicsConfig, wallEast);
            _wallWest.SetGraphic(_tileGraphicsConfig, wallWest);
            _floor.SetGraphic(_tileGraphicsConfig, floor);
            _ceiling.SetGraphic(_tileGraphicsConfig, ceiling);
        }
    }
}
