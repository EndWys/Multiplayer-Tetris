using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
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

        NetworkManager netManager;

        private void Start()
        {
            _instance = this;
            netManager = NetworkManager.Singleton;

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
            if (!netManager.IsServer)
            {
                return;
            }

            if (_isGameStarted)
            {
                return;
            }

            if (_serverStarted)
            {
                if (netManager.ConnectedClientsList.Count == 2)
                {
                    StartMatchClientRpc();
                }
            }
        }

        [ClientRpc]
        public void StartMatchClientRpc() {
            _isGameStarted = true;
            GameScoreScreen.Instance.ResetScore();
            StartGameServerRpc((int)netManager.LocalClientId);
        }


        [ServerRpc(RequireOwnership = false)]
        private void StartGameServerRpc(int clientId)
        {
            _gameControllers[clientId].StartGame(clientId);
        }

        [ClientRpc]
        public void AddPointsClientRpc(int value, int clientId)
        {
            if ((int)netManager.LocalClientId == clientId)
            {
                GameScoreScreen.Instance.AddPoints(value);
            }
        }

        [ClientRpc]
        public void OnGameOverClientRpc(int clientLoser)
        {
            if ((int)netManager.LocalClientId == clientLoser)
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
            GameOverScreen.Instance.HideScreen();
            GameWinnerScreen.Instance.HideScreen();
            GameScoreScreen.Instance.ResetScore();
        }

        public void CreateLineForOtherPlayer(int y,int clientId)
        {
            Debug.Log("Create Line for other");
            var lineReciver = _gameControllers.FirstOrDefault(contorller => contorller.ClientId != clientId);

            lineReciver.WaitMomentToCreateLine(y);
        }
    }
}
