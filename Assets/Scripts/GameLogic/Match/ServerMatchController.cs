using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace TetrisNetwork
{
    public class ServerMatchController : NetworkBehaviour
    {
        [SerializeField] List<GameController> _gameControllers;

        NetworkManager netManager;

        private void Start()
        {
            netManager = NetworkManager.Singleton;
        }

        [ClientRpc]
        public void StartMatchClientRpc() {
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
        public void RestartGameServerRpc()
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
