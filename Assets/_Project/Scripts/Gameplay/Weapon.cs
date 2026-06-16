using Arena.Networking;
using Fusion;
using UnityEngine;

namespace Arena.Gameplay
{
    public class Weapon : NetworkBehaviour
    {
        [SerializeField] private NetworkObject _projectilePrefab;
        [SerializeField] private Transform _firePoint;

        private AmmoSystem _ammo;
        private NetworkButtons _previousButtons;

        public override void Spawned()
        {
            _ammo = GetComponent<AmmoSystem>();
        }

        public override void FixedUpdateNetwork()
        {
            if (!GetInput(out NetworkInputData input))
                return;

            var pressed = input.Buttons.GetPressed(_previousButtons);
            _previousButtons = input.Buttons;

            if (!pressed.IsSet(NetworkInputData.ButtonAttack))
                return;

            if (!_ammo.HasAmmo || !HasStateAuthority)
                return;

            _ammo.ConsumeAmmo();

            // Calcula direção do tiro a partir da posição do mouse no mundo
            var aimWorldPos = new Vector3(input.Aim.x, transform.position.y, input.Aim.y);
            var direction = (aimWorldPos - transform.position).normalized;

            if (direction.sqrMagnitude < 0.01f)
                direction = transform.forward;

            var rotation = Quaternion.LookRotation(direction);
            Runner.Spawn(_projectilePrefab, _firePoint.position, rotation, Object.InputAuthority);
        }
    }
}
