using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TetrisNetwork
{
    public class TetrominoSpawner
    {
        private bool _controledRandom;

        private List<TetrominoSpecs> _allTetriminos = new List<TetrominoSpecs>();
        private List<TetrominoSpecs> _availableTetriminos = new List<TetrominoSpecs>();

        private TetrominoSpecs _oneBlockSpec = OneBlockSpecs();

        public TetrominoSpawner(bool controledRandom, List<TetrominoSpecs> allTetriminos)
        {
            _controledRandom = controledRandom;
            _allTetriminos = allTetriminos;
        }

        public Tetromino GetRandomTetromino()
        {
            if (_controledRandom)
            {
                if (_availableTetriminos.Count == 0)
                {
                    _availableTetriminos = GetFullTetrominoBaseList();
                }

                var tetrominoSpecs = _availableTetriminos[RandomGenerator.random.Next(0, _availableTetriminos.Count)];
                _availableTetriminos.Remove(tetrominoSpecs);
                return new Tetromino(tetrominoSpecs);
            }
         
            return new Tetromino(_allTetriminos[RandomGenerator.random.Next(0, _allTetriminos.Count)]);
        }

        public Tetromino GetOneBlockTetromino()
        {
            return new Tetromino(_oneBlockSpec);
        }

        private List<TetrominoSpecs> GetFullTetrominoBaseList()
        {
            var allTetriminos = new List<TetrominoSpecs>(_allTetriminos);
            return allTetriminos;
        }

        private static TetrominoSpecs OneBlockSpecs()
        {
            TetrominoSpecs oneBlock = new();

            oneBlock.Name = "SingleBlock";
            oneBlock.Color = Color.white;
            oneBlock.InitialPosition = new Vector2Int[Tetromino.BLOCK_ROTATIONS];
            oneBlock.SerializedBlockPositions = new List<int>();


            for (int i = 0; i < Tetromino.BLOCK_ROTATIONS; i++)
            {
                oneBlock.InitialPosition[i] = new Vector2Int(0, 0);

                for (int j = 0; j < Tetromino.BLOCK_AREA; j++)
                {
                    for (int l = 0; l < Tetromino.BLOCK_AREA; l++)
                    {
                        if (j == 0 && l == 0)
                        {
                            oneBlock.SerializedBlockPositions.Add(1);
                        }
                        else
                        {
                            oneBlock.SerializedBlockPositions.Add(0);
                        }
                    }
                }
            }

            return oneBlock;
        }
    }
}
