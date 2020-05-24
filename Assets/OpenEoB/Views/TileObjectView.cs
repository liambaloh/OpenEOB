using UnityEngine;

namespace OpenEoB.Views
{
    public class TileObjectView : MonoBehaviour
    {
        [SerializeField] private AbstractPlayerTileEnterView _playerTileEnterView;
        [SerializeField] private AbstractPlayerTileBumpView _playerTileBumpView;

        protected TileView Tile;

        protected virtual void Start()
        {
            
        }

        public void Setup(TileView tileView)
        {
            Tile = tileView;
        }

        public bool CanPlayerEnterTile()
        {
            return _playerTileEnterView.CanPlayerEnterTile();
        }

        public void Bump()
        {
            _playerTileBumpView.Bump();
        }
    }
}