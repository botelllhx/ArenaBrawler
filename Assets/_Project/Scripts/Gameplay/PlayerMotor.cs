using Arena.Networking;
using Fusion;
using UnityEngine;

namespace Arena.Gameplay
{
    public class PlayerMotor : NetworkBehaviour
    {
        [SerializeField] private float _speed = 6f;

        public override void Spawned()
        {
            if (HasInputAuthority)
            {
                var cam = FindAnyObjectByType<CameraController>();
                if (cam != null)
                    cam.SetTarget(transform);
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData input))
            {
                var moveDirection = new Vector3(input.Move.x, 0f, input.Move.y);
                if (moveDirection.sqrMagnitude > 1f)
                    moveDirection.Normalize();
                transform.position += moveDirection * _speed * Runner.DeltaTime;

                // Rotação aponta para a mira (posição do mouse no mundo)
                var aimWorldPos = new Vector3(input.Aim.x, transform.position.y, input.Aim.y);
                var aimDirection = aimWorldPos - transform.position;
                if (aimDirection.sqrMagnitude > 0.1f)
                    transform.rotation = Quaternion.LookRotation(aimDirection);
            }
        }
    }
}
