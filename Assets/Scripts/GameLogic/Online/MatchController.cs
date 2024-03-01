using Unity.Netcode;
using UnityEngine;

namespace TetrisNetwork
{
    public class MatchController : NetworkBehaviour
    {
        bool _serverStarted = false;
        bool isGameStarted = false;

        [SerializeField] GameController _gameController1;
        [SerializeField] GameController _gameController2;
        private void Start()
        {
            NetworkManager.Singleton.OnServerStarted += () => _serverStarted = true;
        }

        private void Update()
        {
            if (isGameStarted)
            {
                return;
            }

            if (_serverStarted)
            {
                if(NetworkManager.Singleton.ConnectedClientsList.Count == 2)
                {
                    isGameStarted = true;
                    _gameController1.StartGame();
                    StartGameClientRpc();
                }
            }
        }

        [ClientRpc]
        public void StartGameClientRpc() {
            Debug.Log("On Client");
            if (IsHost)
            {
                return;
            }
            Debug.Log("Game 2 start");
            _gameController2.StartGame();
        }
    }
}
