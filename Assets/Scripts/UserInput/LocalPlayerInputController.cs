using System;
using System.Collections.Generic;
using TetrisNetwork;
using Unity.Netcode;
using UnityEngine;
using VContainer;

public class LocalPlayerInputController : MonoBehaviour
{
    [SerializeField] ServerInputController _serverInputController;

    [SerializeField] EditorGameInput _editroGameInput;
    [SerializeField] BuildGameInput _buildGameInput;

    private IGameInput _inputController;

    [Inject]
    public void Constract()
    {
        SetInputController();
        ConnectSignals();
    }

    public void SetInputController()
    {
#if UNITY_EDITOR
        _inputController = _editroGameInput;
#else
        _inputController = _editroGameInput;
#endif
        _inputController.Initialize();

    }

    private void ConnectSignals()
    {
        _inputController.OnInput = DoInput;
    }

    private void DoInput(InputT type)
    {
        Debug.Log("Local input: " + (int)NetworkManager.Singleton.LocalClientId);
        _serverInputController.MakeInputMoveServerRpc(type, (int)NetworkManager.Singleton.LocalClientId);
    }
}
