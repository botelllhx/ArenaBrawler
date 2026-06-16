using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Arena.Networking
{
    [RequireComponent(typeof(PlayerInputCollector))]
    public class NetworkBootstrap : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private NetworkObject _playerPrefab;
        [SerializeField] private int _arenaSceneBuildIndex = 1;

        private NetworkRunner _runner;
        private PlayerInputCollector _inputCollector;
        private readonly Dictionary<PlayerRef, NetworkObject> _spawnedPlayers = new();

        private void Awake()
        {
            _inputCollector = GetComponent<PlayerInputCollector>();
            DontDestroyOnLoad(gameObject);
        }

        public async Task StartAsHost(string sessionName)
        {
            await StartGame(GameMode.Host, sessionName);
        }

        public async Task StartAsClient(string sessionName)
        {
            await StartGame(GameMode.Client, sessionName);
        }

        private async Task StartGame(GameMode mode, string sessionName)
        {
            if (_runner != null)
                return;

            _runner = gameObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = true;

            var sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();

            var sceneInfo = new NetworkSceneInfo();
            sceneInfo.AddSceneRef(SceneRef.FromIndex(_arenaSceneBuildIndex));

            var result = await _runner.StartGame(new StartGameArgs
            {
                GameMode = mode,
                SessionName = sessionName,
                Scene = sceneInfo,
                SceneManager = sceneManager,
            });

            if (!result.Ok)
                Debug.LogError($"Failed to start game: {result.ErrorMessage}");
        }

        // --- INetworkRunnerCallbacks ---

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                var obj = runner.Spawn(_playerPrefab, Vector3.up, Quaternion.identity, player);
                _spawnedPlayers[player] = obj;
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (_spawnedPlayers.TryGetValue(player, out var obj))
            {
                runner.Despawn(obj);
                _spawnedPlayers.Remove(player);
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            var data = _inputCollector.CollectInput();
            input.Set(data);
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    }
}
