using UnityEngine;

namespace Arena.Gameplay
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Vector3 _offset = new(0, 20, 0);

        private Transform _target;

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        private void LateUpdate()
        {
            if (_target != null)
                transform.position = _target.position + _offset;
        }
    }
}
