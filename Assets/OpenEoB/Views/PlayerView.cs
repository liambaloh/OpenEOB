using System;
using UnityEngine;

namespace OpenEoB.Views
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField]private TileView _location;
        private MapView _map;

        public void Teleport(TileView _tileView)
        {
            _location = _tileView;
            this.transform.position = _location.transform.position;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveNorth();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveSouth();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveEast();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveWest();
            }

            var thisPosition2D = new Vector2(this.transform.position.x, this.transform.position.z);
            var locationPosition2D = new Vector2(_location.transform.position.x, _location.transform.position.z);

            if (Vector2.Distance(thisPosition2D, locationPosition2D) > 0.01f)
            {
                var newPosition2D = Vector2.MoveTowards(thisPosition2D, locationPosition2D, 1 * Time.deltaTime);
                this.transform.position = new Vector3(newPosition2D.x, this.transform.position.y, newPosition2D.y);
            }
        }

        internal void Setup(MapView mapView)
        {
            _map = mapView;
        }

        private void Move(int diffX, int diffY)
        {
            if (_map.TileExists(_location.X + diffX, _location.Y + diffY))
            {
                _location = _map.GetTile(_location.X + diffX, _location.Y + diffY);
            }
        }

        public void MoveNorth()
        {
            Move(0, 1);
        }

        public void MoveSouth()
        {
            Move(0, -1);
        }

        public void MoveEast()
        {
            Move(1, 0);
        }

        public void MoveWest()
        {
            Move(-1, 0);
        }
    }
}
