using Fusion;
using UnityEngine;

namespace Arena.Gameplay
{
    public class AmmoSystem : NetworkBehaviour
    {
        [SerializeField] private int _maxAmmo = 3;
        [SerializeField] private float _rechargeTime = 1.5f;

        [Networked] public int CurrentAmmo { get; private set; }
        [Networked] private TickTimer _rechargeTimer { get; set; }

        public bool HasAmmo => CurrentAmmo > 0;

        public override void Spawned()
        {
            if (HasStateAuthority)
                CurrentAmmo = _maxAmmo;
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority)
                return;

            if (CurrentAmmo >= _maxAmmo)
                return;

            if (!_rechargeTimer.IsRunning)
                _rechargeTimer = TickTimer.CreateFromSeconds(Runner, _rechargeTime);

            if (_rechargeTimer.Expired(Runner))
            {
                CurrentAmmo++;
                _rechargeTimer = default;
            }
        }

        public void ConsumeAmmo()
        {
            if (!HasStateAuthority || CurrentAmmo <= 0)
                return;

            CurrentAmmo--;
            if (CurrentAmmo < _maxAmmo)
                _rechargeTimer = TickTimer.CreateFromSeconds(Runner, _rechargeTime);
        }
    }
}
