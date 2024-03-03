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
            GameWinnerScreen.Instance.HideScreen(0f);
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

        [ClientRpc]
        public void StartMatchClientRpc() {
            GameScoreScreen.Instance.ResetScore();
            StartGameServerRpc((int)NetworkManager.LocalClientId);
        }


        [ServerRpc(RequireOwnership = false)]
        private void StartGameServerRpc(int clientId)
        {
            _gameControllers[clientId].StartGame(clientId);
        }

        [ClientRpc]
        public void AddPointsClientRpc(int value, int clientId)
        {
            if ((int)NetworkManager.Singleton.LocalClientId == clientId)
            {
                GameScoreScreen.Instance.AddPoints(value);
            }
        }

        [ClientRpc]
        public void OnGameOverClientRpc(int clientLoser)
        {
            if ((int)NetworkManager.Singleton.LocalClientId == clientLoser)
            {
                GameOverScreen.Instance.ShowScreen();
            } else
            {
                GameWinnerScreen.Instance.ShowScreen();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void OnGameOverServerRpc()
        {
            foreach(var player in _gameControllers) { player.SetGameOver(); }
        }

        public void CallRestart()
        {
            RestartGameServerRpc();
            RestartGameClientRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void RestartGameServerRpc()
        {
            foreach (var player in _gameControllers) { player.RestartGame(); }
        }

        [ClientRpc]
        public void RestartGameClientRpc()
        {
            Debug.Log("Hide Screen");
            GameOverScreen.Instance.HideScreen();
            GameWinnerScreen.Instance.HideScreen();
            GameScoreScreen.Instance.ResetScore();
        }
    }
}
