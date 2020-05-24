using System;
using System.Collections.Generic;
using System.IO;
using OpenEoB.Views;
using UnityEngine;

namespace OpenEoB
{
    public class Initialiser : MonoBehaviour
    {
        [SerializeField] private MapView _mapPrefab;
        [SerializeField] private PlayerView _playerPrefab;

        void Start()
        {
            Initialise();
        }

        private void Initialise()
        {
            var mapView = Instantiate(_mapPrefab);
            mapView.GenerateMap("gladstone");
            
            var playerView = Instantiate(_playerPrefab);
            playerView.Setup(mapView);
        }
    }
}
