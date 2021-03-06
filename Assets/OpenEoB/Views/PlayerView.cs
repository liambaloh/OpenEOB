﻿using System;
using OpenEoB.Views.Items;
using OpenEoB.Views.UI;
using UnityEngine;

namespace OpenEoB.Views
{
    public class PlayerView : MonoBehaviour
    {
        public static PlayerView Instance;

        private const int North = 1;
        private const int South = 2;
        private const int East = 4;
        private const int West = 8;

        private const float MovementSpeed = 10f;
        private const float RotationSpeed = 10f;

        [SerializeField] private TileView _location;
        private MapView _map;
        private ActiveItemView _activeItemView;
        private int _facing = 1;

        private ItemView _activeItem;

        public void Teleport(TileView _tileView)
        {
            _location = _tileView;
            this.transform.position = _location.transform.position;
        }

        private void Awake()
        {
            if (Instance != null)
            {
                throw new Exception("Player Instance is not null");
            }

            Instance = this;
        }

        private void Start()
        {
            DontDestroyOnLoad(this);

            InputManager.Instance.RegisterAction(Command.MoveForward, this, MoveForward);
            InputManager.Instance.RegisterAction(Command.MoveBackward, this, MoveBackward);
            InputManager.Instance.RegisterAction(Command.TurnLeft, this, FaceLeft);
            InputManager.Instance.RegisterAction(Command.TurnRight, this, FaceRight);
            InputManager.Instance.RegisterAction(Command.StrafeLeft, this, MoveLeft);
            InputManager.Instance.RegisterAction(Command.StrafeRight, this, MoveRight);
        }

        private void Update()
        {
            var thisPosition2D = new Vector2(this.transform.position.x, this.transform.position.z);
            var locationPosition2D = new Vector2(_location.transform.position.x, _location.transform.position.z);
            var thisRotation = ClampRotation0To360(this.transform.rotation.eulerAngles.y);
            var facingRotation = ClampRotation0To360(GetRotationForFacing(_facing));

            if (Vector2.Distance(thisPosition2D, locationPosition2D) > 0.01f)
            {
                var newPosition2D =
                    Vector2.MoveTowards(thisPosition2D, locationPosition2D, MovementSpeed * Time.deltaTime);
                this.transform.position = new Vector3(newPosition2D.x, this.transform.position.y, newPosition2D.y);
            }

            if (Mathf.Abs(thisRotation - facingRotation) > 0.01f)
            {
                var differenceInRotation = ClampRotationToSigned180(thisRotation - facingRotation);
                if (differenceInRotation < 0)
                {
                    this.transform.Rotate(0, 90 * RotationSpeed * Time.deltaTime, 0);
                }
                else
                {
                    this.transform.Rotate(0, -90 * RotationSpeed * Time.deltaTime, 0);
                }

                var differenceInRotationAfterRotation =
                    ClampRotationToSigned180(
                        ClampRotation0To360(this.transform.rotation.eulerAngles.y) - facingRotation);
                if (differenceInRotationAfterRotation * differenceInRotation < 0) //sign changed
                {
                    this.transform.rotation = Quaternion.Euler(0, facingRotation, 0);
                }
            }
        }

        public void Setup(MapView mapView, ActiveItemView activeItemView)
        {
            _map = mapView;
            _activeItemView = activeItemView;
        }

        public void SetActiveItem(ItemView item)
        {
            _activeItem = item;
            _activeItemView.SetItemSprite(_activeItem.ItemSprite);
        }

        public void RemoveActiveItem()
        {
            _activeItem = null;
            _activeItemView.RemoveItemSprite();
        }

        public bool HasActiveItem()
        {
            return _activeItem != null;
        }

        public ItemView GetActiveItem()
        {
            return _activeItem;
        }

        #region Movement and Facing

        private float ClampRotation0To360(float rotation)
        {
            var result = rotation - Mathf.CeilToInt(rotation / 360f) * 360f;
            if (result < 0)
            {
                result += 360f;
            }

            return result;
        }

        private float ClampRotationToSigned180(float rotation)
        {
            var result = rotation - Mathf.CeilToInt(rotation / 360f) * 360f;
            if (result < 0)
            {
                result += 360f;
            }

            if (result > 180)
            {
                result -= 360;
            }

            return result;
        }

        private float GetRotationForFacing(int facing)
        {
            switch (facing)
            {
                case 1:
                    return 0f;
                case 2:
                    return 180f;
                case 4:
                    return 90f;
                case 8:
                    return -90f;
                default:
                    throw new Exception("Cannot return rotation for facing: " + facing);
            }
        }

        private void Move(int diffX, int diffY)
        {
            if (_map.TileExists(_location.X + diffX, _location.Y + diffY))
            {
                var movementTargetTile = _map.GetTile(_location.X + diffX, _location.Y + diffY);
                if (movementTargetTile.CanPlayerEnterTile())
                {
                    _location = movementTargetTile;
                }
                else
                {
                    movementTargetTile.Bump();
                }
            }
        }

        private void MoveNorth()
        {
            Move(0, 1);
        }

        private void MoveSouth()
        {
            Move(0, -1);
        }

        private void MoveEast()
        {
            Move(1, 0);
        }

        private void MoveWest()
        {
            Move(-1, 0);
        }

        private void Face(int newFacing)
        {
            _facing = newFacing;
        }

        private void FaceLeft()
        {
            switch (_facing)
            {
                case North:
                    Face(West);
                    break;
                case South:
                    Face(East);
                    break;
                case East:
                    Face(North);
                    break;
                case West:
                    Face(South);
                    break;
            }
        }

        private void FaceRight()
        {
            switch (_facing)
            {
                case North:
                    Face(East);
                    break;
                case South:
                    Face(West);
                    break;
                case East:
                    Face(South);
                    break;
                case West:
                    Face(North);
                    break;
            }
        }

        private void MoveForward()
        {
            switch (_facing)
            {
                case North:
                    MoveNorth();
                    break;
                case South:
                    MoveSouth();
                    break;
                case East:
                    MoveEast();
                    break;
                case West:
                    MoveWest();
                    break;
            }
        }

        private void MoveBackward()
        {
            switch (_facing)
            {
                case North:
                    MoveSouth();
                    break;
                case South:
                    MoveNorth();
                    break;
                case East:
                    MoveWest();
                    break;
                case West:
                    MoveEast();
                    break;
            }
        }

        private void MoveLeft()
        {
            switch (_facing)
            {
                case North:
                    MoveWest();
                    break;
                case South:
                    MoveEast();
                    break;
                case East:
                    MoveNorth();
                    break;
                case West:
                    MoveSouth();
                    break;
            }
        }

        private void MoveRight()
        {
            switch (_facing)
            {
                case North:
                    MoveEast();
                    break;
                case South:
                    MoveWest();
                    break;
                case East:
                    MoveSouth();
                    break;
                case West:
                    MoveNorth();
                    break;
            }
        }

        #endregion
    }
}