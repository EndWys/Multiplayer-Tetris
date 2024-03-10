using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TetrisNetwork
{
    public class LocalMatchStarter : IStartable, ITickable
    {
        private ServerMatchController _serverMatchController;

        private bool _serverStarted = false;
        private bool _isGameStarted = false;

        private NetworkManager _netManager;

        [Inject]
        public LocalMatchStarter(ServerMatchController serverMatchController)
        {
            _serverMatchController = serverMatchController;
        }

        void IStartable.Start()
        {
            _netManager = NetworkManager.Singleton;

            _netManager.OnServerStarted += () => _serverStarted = true;

            HideUI();
        }

        void HideUI()
        {
            GameOverScreen.Instance.HideScreen();
            GameWinnerScreen.Instance.HideScreen();
            FieldArrowScreen.Instance.HideScreen();
            GameScoreScreen.Instance.HideScreen();
        }

        public void Tick()
        {
            WaitForMatchReadyToStart();
        }

        void WaitForMatchReadyToStart()
        {
            if (!_netManager.IsServer)
            {
                return;
            }

            if (_isGameStarted)
            {
                return;
            }

            if (_serverStarted)
            {
                if (_netManager.ConnectedClientsList.Count == 2)
                {
                    _serverMatchController.StartMatchClientRpc();
                    ServerEventSender.Instance.SendEventClientRpc(GameEventType.MatchStart);
                    _isGameStarted = true;
                }
            }
        }

        public void ShowYoursGameField(int clientId)
        {
            _serverMatchController.ShowYoursGameFieldClientRpc(clientId);
        }

        public void CallRestart()
        {
            _serverMatchController.RestartGameServerRpc();
        }

        public void OnDestroyLine(int poitns, int clientId)
        {
            const int LOWES_POINT = GameField.HEIGHT - 1;

            _serverMatchController.AddPointsClientRpc(poitns, clientId);
            _serverMatchController.CreateLineForOtherPlayer(LOWES_POINT, clientId);
        }


        public void OnGameOver(int clientId)
        {
            _serverMatchController.OnGameOverServerRpc();
            _serverMatchController.OnGameOverClientRpc(clientId);
        }
    }
}