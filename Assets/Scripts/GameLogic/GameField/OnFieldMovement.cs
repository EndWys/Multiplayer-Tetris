using System.Collections;
using System.Collections.Generic;
using TetrisNetwork;
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
            Tetromino = tetromino;
            Rotation = rotatio;
            X = x;
            Y = y;
        }

        
    }
}
