using UnityEngine;

namespace OpenEoB.Views
{
    public class FacePlayerView : MonoBehaviour
    {
        private PlayerView _player;

        private void Start()
        {
            _player = FindObjectOfType<PlayerView>();
        }

        private void Update()
        {
            this.transform.LookAt(_player.transform, Vector3.up);
        }

        public void FacePlayer()
        {

        }
    }
}
