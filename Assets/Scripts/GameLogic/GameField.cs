using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TetrisNetwork
{
    public enum SpotState
    {
        Empty = 0,
        Filled = 1
    }
    public class GameField
    {
        public const int WIDTH = 10;
        public const int HEIGHT = 22;

        public Action OnCurrentPieceReachBottom;
        public Action OnGameOver;
        public Action<int> OnDestroyLine;

        private int[][] _gameField = new int[WIDTH][];
        private TetrominoSpawner _spawner;
        private Tetromino _currentTetrimino;
        private GameSettings _gameSettings;

        public GameField(GameSettings gameSettings)
        {
            _gameSettings = gameSettings;

            for (int i = 0; i < WIDTH; i++)
            {
                _gameField[i] = new int[HEIGHT];
            }

            Restart();

            _spawner = new TetrominoSpawner(gameSettings.ControledRandomMode, gameSettings.Pieces);
        }

        public void Restart()
        {
            _currentTetrimino = null;
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    _gameField[i][j] = (int)SpotState.Empty;
                }
            }
        }

        public Tetromino CreateTetromino()
        {
            _currentTetrimino = _spawner.GetRandomTetromino();

            int rotation = RandomGenerator.random.Next(0, _currentTetrimino.BlockPositions.GetLength(0));
            Vector2Int position = _currentTetrimino.GetInitialPosition(rotation);
            position.x += WIDTH / 2;

            _currentTetrimino.CurrentPosition = position;
            _currentTetrimino.CurrentRotation = rotation;

            return _currentTetrimino;
        }

        public void Step()
        {
            Vector2Int position = _currentTetrimino.CurrentPosition;
            int rotation = _currentTetrimino.CurrentRotation;

            if (IsPossibleMovement(position.x, position.y + 1, _currentTetrimino, rotation))
            {
                _currentTetrimino.CurrentPosition = new Vector2Int(position.x, position.y + 1);
            }
            else
            {
                PlaceTetrimino(_currentTetrimino);
                DeletePossibleLines();

                if (IsGameOver())
                {
                    OnGameOver.Invoke();
                    return;
                }

                OnCurrentPieceReachBottom.Invoke();
            }
        }

        public bool IsPossibleMovement(int x, int y, Tetromino tetromino, int rotation)
        {
            for (int i1 = x, i2 = 0; i1 < x + Tetromino.BLOCK_AREA; i1++, i2++)
            {
                for (int j1 = y, j2 = 0; j1 < y + Tetromino.BLOCK_AREA; j1++, j2++)
                {
                    if (i1 < 0 ||
                        i1 > WIDTH - 1 ||
                        j1 > HEIGHT - 1)
                    {
                        if (tetromino.ValidBlock(rotation, j2, i2))
                            return false;
                    }

                    if (j1 >= 0)
                    {
                        if ((tetromino.ValidBlock(rotation, j2, i2)) &&
                            (!IsFreeBlock(i1, j1)))
                            return false;
                    }
                }
            }

            return true;
        }

        private bool IsFreeBlock(int pX, int pY)
        {
            return _gameField[pX][pY] == (int)SpotState.Empty;
        }

        private void PlaceTetrimino(Tetromino tetromino)
        {
            var tetrotminoPosition = tetromino.CurrentPosition;
            for (int i1 = tetrotminoPosition.x, i2 = 0; i1 < tetrotminoPosition.x + Tetromino.BLOCK_AREA; i1++, i2++)
            {
                for (int j1 = tetrotminoPosition.y, j2 = 0; j1 < tetrotminoPosition.y + Tetromino.BLOCK_AREA; j1++, j2++)
                {
                    if (tetromino.ValidBlock(tetromino.CurrentRotation, j2, i2) && InBounds(i1, j1))
                    {
                        _gameField[i1][j1] = (int)SpotState.Filled;
                    }
                }
            }
        }


        private bool InBounds(int x, int y)
        {
            return x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT;
        }

        private void DeletePossibleLines()
        {
            for (int j = 0; j < HEIGHT; j++)
            {
                int i = 0;
                while (i < WIDTH)
                {
                    if (_gameField[i][j] != (int)SpotState.Filled) 
                    {
                        break;
                    }
                    i++;
                }

                if (i == WIDTH) DeleteLine(j);
            }
        }

        private void DeleteLine(int y)
        {
            for (int j = y; j > 0; j--)
            {
                for (int i = 0; i < WIDTH; i++)
                {
                    _gameField[i][j] = _gameField[i][j - 1];
                }
            }

            OnDestroyLine.Invoke(y);
        }

        bool IsGameOver()
        {
            for (int i = 0; i < WIDTH; i++)
            {
                if (_gameField[i][0] == (int)SpotState.Filled)
                    return true;
            }

            return false;
        }
    }
}
