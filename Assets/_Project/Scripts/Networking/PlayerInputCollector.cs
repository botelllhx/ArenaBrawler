using UnityEngine;
using UnityEngine.InputSystem;

namespace Arena.Networking
{
    public class PlayerInputCollector : MonoBehaviour
    {
        [SerializeField] private InputActionReference _moveAction;
        [SerializeField] private InputActionReference _lookAction;

        public NetworkInputData CollectInput()
        {
            var data = new NetworkInputData();
            data.Move = _moveAction.action.ReadValue<Vector2>();
            data.Aim = _lookAction.action.ReadValue<Vector2>();
            return data;
        }

        private void OnEnable()
        {
            _moveAction.action.Enable();
            _lookAction.action.Enable();
        }

        private void OnDisable()
        {
            _moveAction.action.Disable();
            _lookAction.action.Disable();
        }
    }
}
