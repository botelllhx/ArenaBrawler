using Arena.Networking;
using Fusion;
using UnityEngine;

namespace Arena.Gameplay
{
    public class PlayerMotor : NetworkBehaviour
    {
        [SerializeField] private float _speed = 6f;

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData input))
            {
                var moveDirection = new Vector3(input.Move.x, 0f, input.Move.y);
                if (moveDirection.sqrMagnitude > 1f)
                    moveDirection.Normalize();
                transform.position += moveDirection * _speed * Runner.DeltaTime;

                if (moveDirection.sqrMagnitude > 0.01f)
                {
                    transform.rotation = Quaternion.LookRotation(moveDirection);
                }
            }
        }
    }
}
