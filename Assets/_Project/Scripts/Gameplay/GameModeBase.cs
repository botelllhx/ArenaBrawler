using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Arena.Gameplay
{
    public enum GameState { Warmup, Playing, Ended }
    public enum Team { None, Blue, Red }

    public abstract class GameModeBase : NetworkBehaviour
    {
        [SerializeField] private float _warmupDuration = 3f;
        [SerializeField] private float _endedDuration = 5f;

        [Networked] public GameState CurrentState { get; private set; }
        [Networked] private TickTimer _stateTimer { get; set; }

        protected readonly List<PlayerController> Players = new();

        public void RegisterPlayer(PlayerController player)
        {
            if (!Players.Contains(player))
                Players.Add(player);
        }

        public void UnregisterPlayer(PlayerController player)
        {
            Players.Remove(player);
        }

        public override void Spawned()
        {
            if (HasStateAuthority)
            {
                CurrentState = GameState.Warmup;
                _stateTimer = TickTimer.CreateFromSeconds(Runner, _warmupDuration);
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority)
                return;

            switch (CurrentState)
            {
                case GameState.Warmup:
                    if (_stateTimer.Expired(Runner))
                    {
                        CurrentState = GameState.Playing;
                        OnMatchStart();
                    }
                    break;
                case GameState.Playing:
                    UpdatePlaying();
                    break;
                case GameState.Ended:
                    if (_stateTimer.Expired(Runner))
                        RestartMatch();
                    break;
            }
        }

        protected virtual void OnMatchStart() { }

        protected virtual void UpdatePlaying() { }

        protected void EndMatch()
        {
            CurrentState = GameState.Ended;
            _stateTimer = TickTimer.CreateFromSeconds(Runner, _endedDuration);
            OnMatchEnd();
        }

        protected virtual void OnMatchEnd() { }

        private void RestartMatch()
        {
            CurrentState = GameState.Warmup;
            _stateTimer = TickTimer.CreateFromSeconds(Runner, _warmupDuration);
            OnRestart();
        }

        protected virtual void OnRestart() { }

        public virtual void OnPlayerDied(PlayerController player) { }

        public void RespawnPlayer(PlayerController player, Vector3 spawnPoint, float delay)
        {
            StartCoroutine(RespawnCoroutine(player, spawnPoint, delay));
        }

        private System.Collections.IEnumerator RespawnCoroutine(PlayerController player, Vector3 spawnPoint, float delay)
        {
            player.gameObject.SetActive(false);
            yield return new WaitForSeconds(delay);
            if (player != null && player.gameObject != null)
            {
                player.transform.position = spawnPoint;
                player.gameObject.SetActive(true);
                player.GetComponent<Health>().Revive();
            }
        }
    }
}
