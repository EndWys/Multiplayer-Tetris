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

            netManager.OnServerStarted += () => _serverStarted = true;

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
            Debug.Log("ClientId " + clientId);
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

        public void CreateLineForOtherPlayer(int y, int clientId)
        {
            var lineReciver = _gameControllers.FirstOrDefault(contorller => contorller.ClientId != clientId);

            if (lineReciver == null)
            {
                Debug.LogException(new System.Exception("No Player for reviving line with bomb"));
                return;
            }

            lineReciver.WaitMomentToCreateBombLine(y);
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
        }

        [ServerRpc(RequireOwnership = false)]
        private void RestartGameServerRpc()
        {
            ResetGameControllers();
            RestartGameClientRpc();
        }

        private void ResetGameControllers()
        {
            foreach (var player in _gameControllers) { player.RestartGame(); }
        }

        [ClientRpc]
        private void RestartGameClientRpc()
        {
            GameOverScreen.Instance.HideScreen();
            GameWinnerScreen.Instance.HideScreen();
            GameScoreScreen.Instance.ResetScore();
        }
    }
}
