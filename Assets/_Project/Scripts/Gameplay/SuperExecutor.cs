using Arena.Gameplay.Data;
using Arena.Networking;
using Fusion;
using UnityEngine;

namespace Arena.Gameplay
{
    public class SuperExecutor : NetworkBehaviour
    {
        [SerializeField] private Transform _firePoint;

        private SuperMeter _meter;
        private SuperDefinition _super;
        private NetworkButtons _previousButtons;

        public override void Spawned()
        {
            _meter = GetComponent<SuperMeter>();
            _super = GetComponent<PlayerController>().Definition.Super;
        }

        public override void FixedUpdateNetwork()
        {
            if (_super == null)
                return;

            if (!GetInput(out NetworkInputData input))
                return;

            var pressed = input.Buttons.GetPressed(_previousButtons);
            _previousButtons = input.Buttons;

            if (!pressed.IsSet(NetworkInputData.ButtonSuper))
                return;

            if (!_meter.IsReady || !HasStateAuthority)
                return;

            _meter.UseSuper();
            ExecuteSuper(input);
        }

        private void ExecuteSuper(NetworkInputData input)
        {
            switch (_super.Type)
            {
                case SuperType.SpecialShot:
                    FireSpecialShot(input.Aim);
                    break;
                case SuperType.Dash:
                    ExecuteDash(input.Move);
                    break;
                case SuperType.Shield:
                    break;
            }
        }

        private void FireSpecialShot(Vector2 aim)
        {
            var aimWorldPos = new Vector3(aim.x, transform.position.y, aim.y);
            var direction = (aimWorldPos - transform.position).normalized;
            if (direction.sqrMagnitude < 0.01f)
                direction = transform.forward;

            var baseRotation = Quaternion.LookRotation(direction);
            var count = _super.ProjectileCount;
            var spread = _super.SpreadAngle;

            if (count <= 1)
            {
                SpawnSuperProjectile(baseRotation);
                return;
            }

            var halfSpread = spread / 2f;
            var angleStep = spread / (count - 1);

            for (int i = 0; i < count; i++)
            {
                var angle = -halfSpread + angleStep * i;
                var rotation = Quaternion.AngleAxis(angle, Vector3.up) * baseRotation;
                SpawnSuperProjectile(rotation);
            }
        }

        private void SpawnSuperProjectile(Quaternion rotation)
        {
            var damage = _super.Damage;
            var speed = _super.ProjectileSpeed;
            var range = 15f;
            var prefab = _super.ProjectilePrefab != null ? _super.ProjectilePrefab
                : GetComponent<PlayerController>().Definition.Attack.ProjectilePrefab;

            Runner.Spawn(prefab, _firePoint.position, rotation, Object.InputAuthority,
                (runner, obj) =>
                {
                    obj.GetComponent<Projectile>().Init(damage, speed, range, null);
                });
        }

        private void ExecuteDash(Vector2 moveInput)
        {
            var direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
            if (direction.sqrMagnitude < 0.01f)
                direction = transform.forward;

            transform.position += direction * _super.DashDistance;
        }
    }
}
