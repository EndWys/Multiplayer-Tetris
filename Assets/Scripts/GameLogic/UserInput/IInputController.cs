using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TetrisNetwork
{
    public interface IInputController
    {
        bool MakeMoveLeft();
        bool MakeMoveRight();
        bool MakeMoveDown();
        bool MakeRotateLeft();
        bool MakeRotateRight();
    }
}
