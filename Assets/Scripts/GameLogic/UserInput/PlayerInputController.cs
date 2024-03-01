using System;
using UnityEngine;
using UnityEngine.Windows;

namespace TetrisNetwork
{
    public class PlayerInputController : MonoBehaviour
    {
        [SerializeField] EditorGameInput _editroGameInput;
        [SerializeField] BuildGameInput _buildGameInput;

        public Action OnMoveLeft; 
        public Action OnMoveRight; 
        public Action OnMoveDown; 
        public Action OnRotateLeft;
        public Action OnRotateRight;

        IGameInput _inputController;

        public void MakeMoveDown()
        {
            OnMoveDown?.Invoke();
        }

        public void MakeMoveLeft()
        {
            OnMoveLeft?.Invoke();
        }

        public void MakeMoveRight()
        {
           OnMoveRight?.Invoke();
        }

        public void MakeRotateLeft()
        {
            OnRotateLeft.Invoke();
        }

        public void MakeRotateRight()
        {
            OnRotateRight.Invoke();
        }

        public void SetInputController()
        {
#if UNITY_EDITOR
            _inputController = _editroGameInput;
#else
            _inputController = _buildGameInput;
#endif
            ConnectInputSystem();

            _inputController.Initialize();
        }

        public void ConnectInputSystem()
        {
            _inputController.OnMoveLeft = MakeMoveLeft;
            _inputController.OnMoveRight = MakeMoveRight;
            _inputController.OnRotateRight = MakeRotateRight;
            _inputController.OnRotateLeft = MakeRotateLeft;
            _inputController.OnMoveDown = MakeMoveDown;
        }
    }
}