using Arena.Gameplay.Data;
using Arena.Networking;
using Fusion;
using UnityEngine;

namespace Arena.Gameplay
{
    public class Weapon : NetworkBehaviour
    {
        [SerializeField] private Transform _firePoint;

        private AmmoSystem _ammo;
        private SuperMeter _superMeter;
        private AttackDefinition _attack;
        private NetworkButtons _previousButtons;

        public override void Spawned()
        {
            _ammo = GetComponent<AmmoSystem>();
            _superMeter = GetComponent<SuperMeter>();
            _attack = GetComponent<PlayerController>().Definition.Attack;
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
            FireProjectiles(input.Aim);
        }

        private void FireProjectiles(Vector2 aim)
        {
            var aimWorldPos = new Vector3(aim.x, transform.position.y, aim.y);
            var direction = (aimWorldPos - transform.position).normalized;
            if (direction.sqrMagnitude < 0.01f)
                direction = transform.forward;

            var baseRotation = Quaternion.LookRotation(direction);
            var count = _attack.ProjectileCount;
            var spread = _attack.SpreadAngle;

            if (count <= 1)
            {
                SpawnProjectile(baseRotation);
                return;
            }

            var halfSpread = spread / 2f;
            var angleStep = spread / (count - 1);

            for (int i = 0; i < count; i++)
            {
                var angle = -halfSpread + angleStep * i;
                var rotation = Quaternion.AngleAxis(angle, Vector3.up) * baseRotation;
                SpawnProjectile(rotation);
            }
        }

        private void SpawnProjectile(Quaternion rotation)
        {
            var damage = _attack.Damage;
            var speed = _attack.ProjectileSpeed;
            var range = _attack.Range;
            var meter = _superMeter;

            Runner.Spawn(_attack.ProjectilePrefab, _firePoint.position, rotation, Object.InputAuthority,
                (runner, obj) =>
                {
                    obj.GetComponent<Projectile>().Init(damage, speed, range, meter);
                });
        }
    }
}
