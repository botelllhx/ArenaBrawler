using Fusion;
using UnityEngine;

namespace Arena.Gameplay
{
    public class Gem : NetworkBehaviour
    {
        [SerializeField] private float _collectRadius = 1.5f;

        [Networked] public PlayerRef Owner { get; private set; }
        [Networked] public NetworkBool IsCollected { get; private set; }

        private Transform _ownerTransform;

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority)
                return;

            if (IsCollected)
                return;

            // Checa colisão com players para coleta
            var colliders = Physics.OverlapSphere(transform.position, _collectRadius);
            foreach (var col in colliders)
            {
                var player = col.GetComponentInParent<PlayerController>();
                if (player == null || !player.gameObject.activeSelf)
                    continue;

                // Coleta
                Owner = player.Object.InputAuthority;
                IsCollected = true;
                player.GemCount++;
                _ownerTransform = player.transform;
                return;
            }
        }

        public override void Render()
        {
            if (IsCollected)
            {
                // Segue o dono visualmente (ou esconde)
                if (_ownerTransform != null)
                    transform.position = _ownerTransform.position + Vector3.up * 2f;
                else
                    transform.position = Vector3.one * 9999f; // esconde temporariamente
            }
        }

        public void Drop(Vector3 position)
        {
            Owner = default;
            IsCollected = false;
            transform.position = position;
            _ownerTransform = null;
        }
    }
}
