using Fusion;
using UnityEngine;

namespace Arena.Gameplay
{
    public class SuperMeter : NetworkBehaviour
    {
        [Networked] public float CurrentCharge { get; private set; }

        private float _chargeRequired;

        public bool IsReady => CurrentCharge >= _chargeRequired;

        public override void Spawned()
        {
            _chargeRequired = GetComponent<PlayerController>().Definition.SuperChargeRequired;
        }

        public void AddCharge(int damageDealt)
        {
            if (!HasStateAuthority)
                return;

            var wasFull = IsReady;
            CurrentCharge = Mathf.Min(CurrentCharge + damageDealt, _chargeRequired);
            if (!wasFull && IsReady)
                Debug.Log("SUPER PRONTO! Aperte Space para usar.");
        }

        public void UseSuper()
        {
            if (!HasStateAuthority)
                return;

            CurrentCharge = 0;
        }
    }
}
