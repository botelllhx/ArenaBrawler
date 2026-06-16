using Fusion;
using UnityEngine;

namespace Arena.Gameplay
{
    public class GemGrabMode : GameModeBase
    {
        [SerializeField] private GemSpawner _spawner;
        [SerializeField] private int _gemsToWin = 10;
        [SerializeField] private float _victoryHoldTime = 15f;
        [SerializeField] private float _respawnDelay = 3f;
        [SerializeField] private Transform[] _blueSpawnPoints;
        [SerializeField] private Transform[] _redSpawnPoints;

        [Networked] public int BlueGems { get; private set; }
        [Networked] public int RedGems { get; private set; }
        [Networked] private TickTimer _victoryTimer { get; set; }
        [Networked] public Team LeadingTeam { get; private set; }

        protected override void OnMatchStart()
        {
            if (_spawner != null)
                _spawner.StartSpawning();
        }

        protected override void UpdatePlaying()
        {
            RecalculateGems();

            var leading = Team.None;
            if (BlueGems >= _gemsToWin)
                leading = Team.Blue;
            else if (RedGems >= _gemsToWin)
                leading = Team.Red;

            if (leading != Team.None)
            {
                if (LeadingTeam != leading)
                {
                    LeadingTeam = leading;
                    _victoryTimer = TickTimer.CreateFromSeconds(Runner, _victoryHoldTime);
                    Debug.Log($"Time {leading} tem {(leading == Team.Blue ? BlueGems : RedGems)} gemas! Segure por {_victoryHoldTime}s para vencer.");
                }

                if (_victoryTimer.Expired(Runner))
                {
                    Debug.Log($"Time {LeadingTeam} venceu!");
                    EndMatch();
                }
            }
            else
            {
                if (LeadingTeam != Team.None)
                {
                    LeadingTeam = Team.None;
                    _victoryTimer = default;
                }
            }
        }

        private void RecalculateGems()
        {
            int blue = 0, red = 0;
            foreach (var player in Players)
            {
                if (player == null) continue;
                if (player.Team == Team.Blue)
                    blue += player.GemCount;
                else if (player.Team == Team.Red)
                    red += player.GemCount;
            }
            BlueGems = blue;
            RedGems = red;
        }

        public override void OnPlayerDied(PlayerController player)
        {
            if (!HasStateAuthority) return;

            // Dropa gemas no chão
            if (player.GemCount > 0 && _spawner != null)
            {
                _spawner.DropGems(player.GemCount, player.transform.position);
                player.GemCount = 0;
            }

            // Respawn com delay
            var spawnPoint = GetSpawnPoint(player.Team);
            RespawnPlayer(player, spawnPoint, _respawnDelay);
        }

        public Vector3 GetSpawnPoint(Team team)
        {
            var points = team == Team.Blue ? _blueSpawnPoints : _redSpawnPoints;
            if (points == null || points.Length == 0)
                return Vector3.up;
            return points[Random.Range(0, points.Length)].position;
        }

        protected override void OnRestart()
        {
            BlueGems = 0;
            RedGems = 0;
            LeadingTeam = Team.None;
            _victoryTimer = default;

            foreach (var player in Players)
            {
                if (player == null) continue;
                player.GemCount = 0;
                player.transform.position = GetSpawnPoint(player.Team);
                player.gameObject.SetActive(true);
                player.GetComponent<Health>().Revive();
            }

            if (_spawner != null)
                _spawner.Reset();
        }
    }
}
