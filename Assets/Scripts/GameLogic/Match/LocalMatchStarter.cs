using TetrisNetwork;
using Unity.Netcode;
using UnityEngine;
using VContainer;

public class LocalMatchStarter : MonoBehaviour
{
    [SerializeField] ServerMatchController _serverMatchController;

    private bool _serverStarted = false;
    private bool _isGameStarted = false;

    private NetworkManager _netManager;

    [Inject]
    public void Construct()
    {

    }

    private void Start()
    {
        _netManager = NetworkManager.Singleton;

        _netManager.OnServerStarted += () => _serverStarted = true;

        GameOverScreen.Instance.HideScreen();
        GameWinnerScreen.Instance.HideScreen();
        FieldArrowScreen.Instance.HideScreen();
        GameScoreScreen.Instance.HideScreen();
    }

    private void Update()
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
        _serverMatchController.AddPointsClientRpc(poitns, clientId);
        _serverMatchController.CreateLineForOtherPlayer(GameField.HEIGHT - 1, clientId);
    }

    public void OnGameOver(int clientId)
    {
        _serverMatchController.OnGameOverServerRpc();
        _serverMatchController.OnGameOverClientRpc(clientId);
    }
}
