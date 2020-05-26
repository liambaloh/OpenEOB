using UnityEngine;
using UnityEngine.UI;

namespace OpenEoB.Views.Items
{
    public class ActiveItemView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        private bool _trackingCursor;
        private RectTransform _rectTransform;

        public void SetItemSprite(Sprite itemSprite)
        {
            _image.sprite = itemSprite;
            _image.color = Color.white;
            _trackingCursor = true;
        }

        public void RemoveItemSprite()
        {
            _image.sprite = null;
            _image.color = new Color(0, 0, 0, 0);
            _trackingCursor = false;
        }

        public void Setup()
        {
            _rectTransform = this.GetComponent<RectTransform>();
            RemoveItemSprite();
        }

        private void Update()
        {
            if (_trackingCursor)
            {
                _rectTransform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            }
        }
    }
}