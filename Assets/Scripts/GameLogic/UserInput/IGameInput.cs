using System;

namespace TetrisNetwork
{
    public enum InputT
    {
        RotateLeft,
        RotateRight,
        MoveLeft,
        MoveRight,
        MoveDown,
    }

    public interface IGameInput
    {
        public Action<InputT> OnInput { get; set; }

        public void Initialize();
    }
}
