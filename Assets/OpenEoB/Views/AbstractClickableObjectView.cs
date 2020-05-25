using UnityEngine;

namespace OpenEoB.Views
{
    public abstract class AbstractClickableObjectView : MonoBehaviour
    {
        private void OnMouseDown()
        {
            UnityEngine.Debug.Log("Click!");
            Clicked();
        }

        protected abstract void Clicked();
    }
}
