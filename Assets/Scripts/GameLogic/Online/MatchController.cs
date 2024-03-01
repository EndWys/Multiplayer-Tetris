using Unity.Netcode;
using UnityEngine;

namespace TetrisNetwork
{
    public class MatchController : NetworkBehaviour
    {
        bool _serverStarted = false;
        bool isGameStarted = false;
        int _usersOnMatchCount = 0;

        [SerializeField] GameController _gameController;
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
                    _gameController.StartGame();
                }
            }
        }
    }
}
