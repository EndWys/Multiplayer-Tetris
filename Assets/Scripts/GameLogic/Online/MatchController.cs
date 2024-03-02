using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace TetrisNetwork
{
    public class MatchController : NetworkBehaviour
    {
        private static MatchController _instance;
        public static MatchController Instance => _instance;

        private bool _serverStarted = false;
        private bool _isGameStarted = false;

        [SerializeField] List<GameController> _gameControllers;

        private void Start()
        {
            _instance = this;

            NetworkManager.Singleton.OnServerStarted += () => _serverStarted = true;

            GameOverScreen.Instance.HideScreen(0f);
            GameScoreScreen.Instance.HideScreen();
        }

        private void Update()
        {
            WaitForMatchReadyToStart();
        }

        void WaitForMatchReadyToStart()
        {
            if (_isGameStarted)
            {
                return;
            }

            if (_serverStarted)
            {
                if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
                {
                    _isGameStarted = true;
                    StartMatchClientRpc();
                }
            }
        }

        public void StartMatch()
        {
            StartGameServerRpc((int)NetworkManager.LocalClientId);
        }

        [ClientRpc]
        public void StartMatchClientRpc() {
            StartGameServerRpc((int)NetworkManager.LocalClientId);
        }


        [ServerRpc(RequireOwnership = false)]
        private void StartGameServerRpc(int clientId)
        {
            _gameControllers[clientId].StartGame(clientId);
        }

        [ClientRpc]
        public void AddPointsClientRpc(int value)
        {
            GameScoreScreen.Instance.AddPoints(value);
        }

        [ServerRpc(RequireOwnership = false)]
        public void OnGameOverServerRpc()
        {
            foreach(var player in _gameControllers) { player.SetGameOver(); }
        }

        public void CallRestart()
        {
            RestartGameServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void RestartGameServerRpc()
        {
            foreach (var player in _gameControllers) { player.RestartGame(); }
        }
    }
}
