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
        private bool isGameStarted = false;

        [SerializeField] GameController _gameController1;
        [SerializeField] GameController _gameController2;
        private void Start()
        {
            _instance = this;

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
                    StartGameClientRpc();
                }
            }
        }

        [ClientRpc]
        public void StartGameClientRpc() {
            Debug.Log("On Client");
            Debug.Log("Client Id - " + NetworkManager.LocalClientId);

            if(NetworkManager.LocalClientId == 0)
            {
                _gameController1.StartGame();
            } else if(NetworkManager.LocalClientId == 1)
            {
                _gameController2.StartGame();
            }
        }

        PoolingObject _objectToSpawn;

        public void SpawnObject(PoolingObject spawnObj)
        {
            _objectToSpawn = spawnObj;
            SpawnObjectServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnObjectServerRpc()
        {
            Debug.Log("Spawn Object");
            _objectToSpawn.GetComponent<NetworkObject>().Spawn(true);
        }
    }
}
