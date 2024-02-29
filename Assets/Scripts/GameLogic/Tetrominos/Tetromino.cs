using System;
using System.Collections.Generic;
using UnityEngine;

namespace TetrisNetwork
{
    public struct TetrominoSpecs
    {
        public string Name;
        public Color Color;
        public List<int> SerializedBlockPositions;
        public Vector2Int[] InitialPosition;
    }

    public class Tetromino
    {
        public const int BLOCK_AREA = 5;
        public const int BLOCK_ROTATIONS = 4;

        public Action OnChangePosition;
        public Action OnChangeRotation;

        public bool IsLocked;

        public string Name { get; private set; }
        public Color Color { get; private set; }

        public int[][][] BlockPositions { get; private set; }

        private Vector2Int[] _initialPosition;
        private Vector2Int _currentPosition;

        public Vector2Int CurrentPosition
        {
            set
            {
                _currentPosition = value;
                if (OnChangePosition != null && !IsLocked)
                    OnChangePosition.Invoke();
            }
            get
            {
                return _currentPosition;
            }
        }

        private int _currentRotation;
        public int CurrentRotation
        {
            set
            {
                _currentRotation = value;
                if (OnChangeRotation != null)
                    OnChangeRotation.Invoke();
            }
            get
            {
                return _currentRotation;
            }
        }

        public int NextRotation { get { return CurrentRotation + 1 > 3 ? 0 : CurrentRotation + 1; } }
        public int PreviousRotation { get { return CurrentRotation - 1 < 0 ? 3 : CurrentRotation - 1; } }


        public Tetromino(TetrominoSpecs specs)
        {
            Name = specs.Name;
            Color = specs.Color;
            _initialPosition = specs.InitialPosition;

            var serPos = specs.SerializedBlockPositions;

            if (serPos.Count != BLOCK_ROTATIONS * BLOCK_AREA * BLOCK_AREA)
            {
                LogGridOfPieceExeption();
                return;
            }

            int position = 0;

            BlockPositions = new int[BLOCK_ROTATIONS][][];

            for (int r = 0; r < BlockPositions.Length; r++)
            {
                BlockPositions[r] = new int[BLOCK_AREA][];

                for (int w = 0; w < BlockPositions[r].Length; w++)
                {
                    BlockPositions[r][w] = new int[BLOCK_AREA];

                    for (int h = 0; h < BlockPositions[r][w].Length; h++)
                    {
                        if (serPos[position] != (int)SpotState.Empty &&
                            serPos[position] != (int)SpotState.Filled)
                        {
                            LogBlockStateExeption(serPos[position]);
                            return;
                        }

                        BlockPositions[r][w][h] = serPos[position++];
                    }
                }
            }
        }

        public Vector2Int GetInitialPosition(int rotation)
        {
            return _initialPosition[rotation];
        }

        public bool ValidBlock(int rotation, int x, int y)
        {
            return BlockPositions[rotation][x][y] != 0;
        }

        void LogGridOfPieceExeption()
        {
            Debug.LogException(new Exception(string.Format(
                           "The layout of piece {0} is wrong. It must have {1} rotations of {2}x{3} grid.",
                            Name, BLOCK_ROTATIONS, BLOCK_AREA, BLOCK_AREA)));
        }

        void LogBlockStateExeption(int pos)
        {
            Debug.LogException(new Exception(string.Format(
                "The layout of piece {0} is wrong in Json file. It contains '{1}' when only {2}s and {3}s are supported.",
                Name, pos, (int)SpotState.Empty, (int)SpotState.Filled)));
        }

       public static TetrominoSpecs TemplateSpec = new TetrominoSpecs {
            Name = "Def",
            Color = Color.red,
            InitialPosition = new[] { 
                new Vector2Int(-2,-2),
                new Vector2Int(-2,-3),
                new Vector2Int(-2,-2),
                new Vector2Int(-2,-3)
            },
            SerializedBlockPositions = new(){0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                1,
                1,
                1,
                1,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                1,
                0,
                0,
                0,
                0,
                1,
                0,
                0,
                0,
                0,
                1,
                0,
                0,
                0,
                0,
                1,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                1,
                1,
                1,
                1,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                1,
                0,
                0,
                0,
                0,
                1,
                0,
                0,
                0,
                0,
                1,
                0,
                0,
                0,
                0,
                1,
                0,
                0,
                0,
                0,
                0,
                0,
                0}
        };
    }
}
