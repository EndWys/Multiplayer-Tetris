using System;

namespace TetrisNetwork
{
    public class PlayerInput
    {
        public Action OnMoveLeft; 
        public Action OnMoveRight; 
        public Action OnMoveDown; 
        public Action OnRotateLeft;
        public Action OnRotateRight;

        IInputController _inputController;

        public bool MakeMoveDown()
        {
            return _inputController.MakeMoveDown();
        }

        public bool MakeMoveLeft()
        {
            return _inputController.MakeMoveLeft();
        }

        public bool MakeMoveRight()
        {
           return _inputController.MakeMoveLeft();
        }

        public bool MakeRotateLeft()
        {
            return _inputController.MakeRotateLeft();
        }

        public bool MakeRotateRight()
        {
            return _inputController.MakeRotateRight();
        }

        public void SetInputController(IInputController input)
        {
            _inputController = input;
        }


    }
}