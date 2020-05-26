using OpenEoB.Views;
using OpenEoB.Views.Items;
using UnityEngine;

namespace OpenEoB
{
    public class Initialiser : MonoBehaviour
    {
        [SerializeField] private MapView _mapPrefab;
        [SerializeField] private PlayerView _playerPrefab;
        [SerializeField] private ActiveItemView _activeItemPrefab;
        [SerializeField] private RectTransform _activeItemParent;
        [SerializeField] private InventoryView _inventoryPrefab;
        [SerializeField] private RectTransform _inventoryParent;

        private void Start()
        {
            Initialise();
        }

        private void Initialise()
        {
            //3D World
            var mapView = Instantiate(_mapPrefab);
            var playerView = Instantiate(_playerPrefab);

            //UI
            var inventoryView = Instantiate(_inventoryPrefab, _inventoryParent);
            var activeItemView = Instantiate(_activeItemPrefab, _activeItemParent);

            mapView.GenerateMap("gladstone");
            activeItemView.Setup();
            playerView.Setup(mapView, activeItemView);
            inventoryView.Setup(playerView);
        }
    }
}
