using Arena.Gameplay.Data;
using Fusion;
using UnityEngine;

namespace Arena.Gameplay
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private BrawlerDefinition _definition;

        [Networked] public Team Team { get; set; }
        [Networked] public int GemCount { get; set; }

        public BrawlerDefinition Definition => _definition;

        private static int _playerCount;

        public override void Spawned()
        {
            if (HasStateAuthority)
            {
                // Atribui time alternando
                Team = _playerCount % 2 == 0 ? Team.Blue : Team.Red;
                _playerCount++;

                // Registra no GameMode e posiciona no spawn point
                var gameMode = FindAnyObjectByType<GemGrabMode>();
                if (gameMode != null)
                {
                    gameMode.RegisterPlayer(this);
                    transform.position = gameMode.GetSpawnPoint(Team);
                }
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            var gameMode = FindAnyObjectByType<GameModeBase>();
            gameMode?.UnregisterPlayer(this);
        }

        public void SetDefinition(BrawlerDefinition definition)
        {
            _definition = definition;
        }
    }
}
