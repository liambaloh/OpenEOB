using System;
using OpenEoB.Events;
using OpenEoB.Views.PlayerTileEnterViews;
using UnityEngine;

namespace OpenEoB.Views.TileObjectActions
{
    public class DoorOpenEventTileObjectAction : AbstractTileObjectAction
    {
        private const float DoorOpenSpeed = 1;

        [SerializeField] private Transform _doorParent;
        [SerializeField] private PlayerTileToggleableEnterView _togglableEnterView;

        [SerializeField] private DoorState _doorState;
        [SerializeField] private EventId _doorUnlockEventId;

        private void Start()
        {
            EventManager.Instance.RegisterAction(_doorUnlockEventId, this, PerformAction);
        }

        private void Update()
        {
            switch (_doorState)
            {
                case DoorState.Opening:
                    _doorParent.Translate(0, DoorOpenSpeed * Time.deltaTime, 0);
                    if (_doorParent.position.y >= 1f)
                    {
                        var doorParentPosition = _doorParent.position;
                        doorParentPosition.y = 1f;
                        _doorParent.position = doorParentPosition;

                        _doorState = DoorState.Open;
                        _togglableEnterView.SetCanPlayerEnterView(true);
                    }

                    break;
                case DoorState.Closing:
                    _doorParent.Translate(0, -DoorOpenSpeed * Time.deltaTime, 0);
                    if (_doorParent.position.y <= 0f)
                    {
                        var doorParentPosition = _doorParent.position;
                        doorParentPosition.y = 0f;
                        _doorParent.position = doorParentPosition;

                        _doorState = DoorState.Closed;
                    }

                    break;
            }
        }

        public override void PerformAction()
        {
            switch (_doorState)
            {
                case DoorState.Closed:
                    _doorState = DoorState.Opening;
                    break;
                case DoorState.Open:
                    _doorState = DoorState.Closing;
                    _togglableEnterView.SetCanPlayerEnterView(false);
                    break;
                case DoorState.Opening:
                case DoorState.Closing:
                    break;
            }
        }

        private enum DoorState
        {
            Open = 1,
            Opening = 2,
            Closed = 3,
            Closing = 4
        }
    }
}