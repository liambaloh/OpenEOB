using UnityEngine;

namespace OpenEoB.Views
{
    public abstract class AbstractClickableObjectView : MonoBehaviour
    {
        private void OnMouseDown()
        {
            Clicked();
        }

        protected abstract void Clicked();
    }
}
