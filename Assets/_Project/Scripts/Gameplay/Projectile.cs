using Fusion;
using UnityEngine;

namespace Arena.Gameplay
{
    public class Projectile : NetworkBehaviour
    {
        [SerializeField] private float _speed = 20f;
        [SerializeField] private int _damage = 1000;
        [SerializeField] private float _lifetime = 2f;
        [SerializeField] private float _hitRadius = 0.5f;
        [SerializeField] private LayerMask _hitMask = -1;

        [Networked] private TickTimer _lifeTimer { get; set; }

        public override void Spawned()
        {
            if (HasStateAuthority)
                _lifeTimer = TickTimer.CreateFromSeconds(Runner, _lifetime);
        }

        public override void FixedUpdateNetwork()
        {
            if (_lifeTimer.Expired(Runner))
            {
                if (HasStateAuthority)
                    Runner.Despawn(Object);
                return;
            }

            transform.position += transform.forward * _speed * Runner.DeltaTime;

            if (!HasStateAuthority)
                return;

            var colliders = Physics.OverlapSphere(transform.position, _hitRadius, _hitMask);
            foreach (var col in colliders)
            {
                var health = col.GetComponentInParent<Health>();
                if (health == null)
                    continue;

                if (health.Object.InputAuthority == Object.InputAuthority)
                    continue;

                health.TakeDamage(_damage);
                Runner.Despawn(Object);
                return;
            }
        }
    }
}
