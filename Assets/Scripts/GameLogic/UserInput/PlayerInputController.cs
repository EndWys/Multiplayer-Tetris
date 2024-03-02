using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Windows;

namespace TetrisNetwork
{
    public class PlayerInputController : NetworkBehaviour
    {
        [SerializeField] EditorGameInput _editroGameInput;
        [SerializeField] BuildGameInput _buildGameInput;

        public Action OnMoveLeft; 
        public Action OnMoveRight; 
        public Action OnMoveDown; 
        public Action OnRotateLeft;
        public Action OnRotateRight;

        IGameInput _inputController;

        int _clientId;

        [ServerRpc(RequireOwnership = false)]
        public void MakeMoveDownServerRpc(int clientId)
        {
            if (!ValidateClient(clientId)) return;
            OnMoveDown?.Invoke();
        }

        [ServerRpc(RequireOwnership = false)]
        public void MakeMoveLeftServerRpc(int clientId)
        {
            if (!ValidateClient(clientId)) return;
            OnMoveLeft?.Invoke();
        }

        [ServerRpc(RequireOwnership = false)]
        public void MakeMoveRightServerRpc(int clientId)
        {
            if (!ValidateClient(clientId)) return;
            OnMoveRight?.Invoke();
        }

        [ServerRpc(RequireOwnership = false)]
        public void MakeRotateLeftServerRpc(int clientId)
        {
            if (!ValidateClient(clientId)) return;
            OnRotateLeft.Invoke();
        }

        [ServerRpc(RequireOwnership = false)]
        public void MakeRotateRightServerRpc(int clientId)
        {
            if (!ValidateClient(clientId)) return;
            OnRotateRight.Invoke();
        }

        public void SetInputController(int clientId)
        {
            Debug.Log("InputControllerId - " + clientId);
            _clientId = clientId;

#if UNITY_EDITOR
            _inputController = _editroGameInput;
#else
            _inputController = _editroGameInput;
#endif
            ConnectInputSystem();

            _inputController.Initialize();
        }

        public void ConnectInputSystem()
        {
            _inputController.OnMoveLeft = () => MakeMoveLeftServerRpc((int)NetworkManager.LocalClientId);
            _inputController.OnMoveRight = () => MakeMoveRightServerRpc((int)NetworkManager.LocalClientId);
            _inputController.OnRotateRight = () => MakeRotateRightServerRpc((int)NetworkManager.LocalClientId);
            _inputController.OnRotateLeft = () => MakeRotateLeftServerRpc((int)NetworkManager.LocalClientId);
            _inputController.OnMoveDown = () => MakeMoveDownServerRpc((int)NetworkManager.LocalClientId);
        }

        bool ValidateClient(int idFromRpc)
        {
            return idFromRpc == _clientId;
        }
    }
}