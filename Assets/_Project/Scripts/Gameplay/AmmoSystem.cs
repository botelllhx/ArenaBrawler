using Fusion;
using UnityEngine;

namespace Arena.Gameplay
{
    public class AmmoSystem : NetworkBehaviour
    {
        [Networked] public int CurrentAmmo { get; private set; }
        [Networked] private TickTimer _rechargeTimer { get; set; }

        private int _maxAmmo;
        private float _rechargeTime;

        public bool HasAmmo => CurrentAmmo > 0;

        public override void Spawned()
        {
            var def = GetComponent<PlayerController>().Definition;
            _maxAmmo = def.MaxAmmo;
            _rechargeTime = def.RechargeTimePerCharge;

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
