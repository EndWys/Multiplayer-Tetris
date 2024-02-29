using UnityEngine;

namespace TetrisNetwork
{
    public class EditorInputController : IInputController
    {
        public bool MakeMoveDown()
        {
            return Input.GetKey(KeyCode.S);
        }

        public bool MakeMoveLeft()
        {
            return Input.GetKeyDown(KeyCode.A);
        }

        public bool MakeMoveRight()
        {
            return Input.GetKeyDown(KeyCode.D);
        }

        public bool MakeRotateLeft()
        {
            return Input.GetKeyDown(KeyCode.LeftArrow);
        }

        public bool MakeRotateRight()
        {
            return Input.GetKeyDown(KeyCode.RightArrow);
        }
    }
}
