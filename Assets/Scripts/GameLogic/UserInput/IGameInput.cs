using System;

namespace TetrisNetwork
{
    public interface IGameInput
    {
        public Action OnMoveLeft { get; set; }
        public Action OnMoveRight { get; set; }
        public Action OnMoveDown { get; set; }
        public Action OnRotateLeft { get; set; }
        public Action OnRotateRight { get; set; }

        public void Initialize();
    }
}
