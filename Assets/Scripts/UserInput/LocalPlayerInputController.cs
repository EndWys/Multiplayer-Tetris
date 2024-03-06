using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace TetrisNetwork {
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
        _inputController = _buildGameInput;
#endif
            _inputController.Initialize();

        }

        private void ConnectSignals()
        {
            _inputController.OnInput = DoInput;
        }

        private void DoInput(InputT type)
        {
            _serverInputController.MakeInputMoveServerRpc(type, (int)NetworkManager.Singleton.LocalClientId);
        }
    }
}
