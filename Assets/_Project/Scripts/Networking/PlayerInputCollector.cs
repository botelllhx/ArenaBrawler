using UnityEngine;
using UnityEngine.InputSystem;

namespace Arena.Networking
{
    public class PlayerInputCollector : MonoBehaviour
    {
        [SerializeField] private InputActionReference _moveAction;
        [SerializeField] private InputActionReference _attackAction;

        public NetworkInputData CollectInput()
        {
            var data = new NetworkInputData();
            data.Move = _moveAction.action.ReadValue<Vector2>();
            data.Buttons.Set(NetworkInputData.ButtonAttack, _attackAction.action.IsPressed());

            var cam = Camera.main;
            if (cam != null && Mouse.current != null)
            {
                var mousePos = Mouse.current.position.ReadValue();
                var ray = cam.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0));
                var plane = new Plane(Vector3.up, Vector3.zero);
                if (plane.Raycast(ray, out float distance))
                {
                    var worldPos = ray.GetPoint(distance);
                    data.Aim = new Vector2(worldPos.x, worldPos.z);
                }
            }

            return data;
        }

        private void OnEnable()
        {
            _moveAction.action.Enable();
            _attackAction.action.Enable();
        }

        private void OnDisable()
        {
            _moveAction.action.Disable();
            _attackAction.action.Disable();
        }
    }
}
