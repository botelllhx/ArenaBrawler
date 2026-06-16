using Fusion;
using UnityEngine;

namespace Arena.Gameplay
{
    public class Projectile : NetworkBehaviour
    {
        [SerializeField] private float _hitRadius = 0.5f;
        [SerializeField] private LayerMask _hitMask = -1;

        [Networked] private TickTimer _lifeTimer { get; set; }

        private float _speed;
        private int _damage;
        private float _range;
        private SuperMeter _ownerSuperMeter;
        private bool _initialized;

        public void Init(int damage, float speed, float range, SuperMeter ownerSuperMeter)
        {
            _damage = damage;
            _speed = speed;
            _range = range;
            _ownerSuperMeter = ownerSuperMeter;
        }

        public override void Spawned()
        {
            if (HasStateAuthority && !_initialized)
            {
                var lifetime = _speed > 0 ? _range / _speed : 2f;
                _lifeTimer = TickTimer.CreateFromSeconds(Runner, lifetime);
                _initialized = true;
            }
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

                if (_ownerSuperMeter != null)
                    _ownerSuperMeter.AddCharge(_damage);

                Runner.Despawn(Object);
                return;
            }
        }
    }
}
