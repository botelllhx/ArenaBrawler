using System;
using Fusion;
using UnityEngine;

namespace Arena.Gameplay
{
    public class Health : NetworkBehaviour
    {
        [SerializeField] private int _maxHealth = 3800;

        [Networked] public int CurrentHealth { get; private set; }

        public event Action OnDeath;

        public override void Spawned()
        {
            if (HasStateAuthority)
                CurrentHealth = _maxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (!HasStateAuthority)
                return;

            CurrentHealth = Mathf.Max(0, CurrentHealth - damage);

            if (CurrentHealth <= 0)
                Die();
        }

        private void Die()
        {
            OnDeath?.Invoke();
            // Fase 2: respawn simples — reposiciona e restaura vida
            transform.position = Vector3.up;
            CurrentHealth = _maxHealth;
        }
    }
}
