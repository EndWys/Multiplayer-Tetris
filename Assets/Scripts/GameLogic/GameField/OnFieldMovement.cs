using UnityEngine;

namespace TetrisNetwork
{
    public class OnFieldMovement
    {
        public Tetromino Tetromino;
        public int Rotation;
        public int X;
        public int Y;

        public OnFieldMovement(Tetromino tetromino, int rotatio, int x, int y)
        {
            RectTransform d;

            Tetromino = tetromino;
            Rotation = rotatio;
            X = x;
            Y = y;
        }
    }
}
