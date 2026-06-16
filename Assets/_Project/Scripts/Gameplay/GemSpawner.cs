using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Arena.Gameplay
{
    public class GemSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkObject _gemPrefab;
        [SerializeField] private float _spawnInterval = 7f;
        [SerializeField] private int _maxGems = 20;

        [Networked] private TickTimer _spawnTimer { get; set; }

        private readonly List<Gem> _activeGems = new();
        private bool _spawning;

        public void StartSpawning()
        {
            _spawning = true;
            _spawnTimer = TickTimer.CreateFromSeconds(Runner, _spawnInterval);
            // Spawna primeira gema imediatamente
            SpawnGem(transform.position);
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority || !_spawning)
                return;

            if (_spawnTimer.Expired(Runner) && _activeGems.Count < _maxGems)
            {
                SpawnGem(transform.position + Random.insideUnitSphere * 2f);
                _spawnTimer = TickTimer.CreateFromSeconds(Runner, _spawnInterval);
            }
        }

        private void SpawnGem(Vector3 position)
        {
            position.y = 0.5f;
            var obj = Runner.Spawn(_gemPrefab, position, Quaternion.identity);
            var gem = obj.GetComponent<Gem>();
            _activeGems.Add(gem);
        }

        public void DropGems(int count, Vector3 position)
        {
            for (int i = 0; i < count; i++)
            {
                var offset = Random.insideUnitSphere * 1.5f;
                offset.y = 0;
                var dropPos = position + offset;
                dropPos.y = 0.5f;

                // Tenta reusar gemas coletadas do jogador
                bool reused = false;
                foreach (var gem in _activeGems)
                {
                    if (gem != null && gem.IsCollected && gem.Owner == default)
                    {
                        gem.Drop(dropPos);
                        reused = true;
                        break;
                    }
                }

                if (!reused)
                    SpawnGem(dropPos);
            }
        }

        public void Reset()
        {
            _spawning = false;
            foreach (var gem in _activeGems)
            {
                if (gem != null && gem.Object != null)
                    Runner.Despawn(gem.Object);
            }
            _activeGems.Clear();
        }
    }
}
