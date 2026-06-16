using System;
using Fusion;
using UnityEngine;

namespace Arena.Gameplay
{
    public class Health : NetworkBehaviour
    {
        [Networked] public int CurrentHealth { get; private set; }

        private int _maxHealth;

        public event Action OnDeath;

        public override void Spawned()
        {
            var def = GetComponent<PlayerController>().Definition;
            _maxHealth = def.MaxHealth;

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

            // Notifica o GameMode se existir
            var gameMode = FindAnyObjectByType<GameModeBase>();
            if (gameMode != null)
            {
                gameMode.OnPlayerDied(GetComponent<PlayerController>());
            }
            else
            {
                // Fallback: respawn simples sem GameMode
                transform.position = Vector3.up;
                CurrentHealth = _maxHealth;
            }
        }

        public void Revive()
        {
            if (HasStateAuthority)
                CurrentHealth = _maxHealth;
        }
    }
}
