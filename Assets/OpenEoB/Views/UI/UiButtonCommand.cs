using UnityEngine;
using UnityEngine.UI;

namespace OpenEoB.Views.UI
{
    public class UiButtonCommand : MonoBehaviour
    {
        [SerializeField] private Command _command;
        [SerializeField] private Button _button;

        private void Start()
        {
            _button.onClick.AddListener(TriggerCommand);
            InputManager.Instance.RegisterAction(_command, this, DisplayClick);
        }

        private void Update()
        {
            _button.targetGraphic.color = Color.Lerp(_button.targetGraphic.color, Color.white, 2 * Time.deltaTime);
        }

        public void TriggerCommand()
        {
            InputManager.Instance.TriggerCommand(_command);
        }

        public void DisplayClick()
        {
            _button.targetGraphic.color = Color.grey;
        }

        private void OnDestroy()
        {
            InputManager.Instance.UnregisterActionsForOwner(this);
        }
    }
}