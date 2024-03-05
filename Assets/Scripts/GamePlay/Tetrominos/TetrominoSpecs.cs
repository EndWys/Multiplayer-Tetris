using System;
using System.Collections.Generic;
using UnityEngine;

namespace TetrisNetwork
{
    [Serializable]
    public struct TetrominoSpecs
    {
        public string Name;
        public Color Color;
        public List<int> SerializedBlockPositions;
        public Vector2Int[] InitialPosition;
    }
}
