using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace TetrisNetwork
{
    public enum SpotState
    {
        Empty = 0,
        Filled = 1,
        Bomb = 2,
    }
    public class GameField
    {
        public const int WIDTH = 10;
        public const int HEIGHT = 22;

        public Action OnCurrentPieceReachBottom;
        public Action OnGameOver;
        public Action OnMomentToCreateLine;
        public Action OnMomentForDetanateBomb;
        public Action<int> OnDestroyLine;

        private int[][] _gameField = new int[WIDTH][];
        private TetrominoSpawner _spawner;
        private Tetromino _currentTetrimino;

        public GameField(GameSettings gameSettings)
        {
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

        public Tetromino CreateOneBlockTetromino()
        {
            return _spawner.GetOneBlockTetromino();
        }

        public void Step()
        {
            Vector2Int position = _currentTetrimino.CurrentPosition;
            int rotation = _currentTetrimino.CurrentRotation;

            var move = new OnFieldMovement(_currentTetrimino, rotation, position.x, position.y + 1);

            if (IsPossibleMovement(move))
            {
                MakeMove(move);
            }
            else
            {
                PlaceTetrimino(_currentTetrimino);
                OnMomentForDetanateBomb?.Invoke();
                DeletePossibleLines();

                if (IsGameOver())
                {
                    OnGameOver?.Invoke();
                    return;
                }

                OnCurrentPieceReachBottom?.Invoke();
            }

            OnMomentToCreateLine?.Invoke();
        }

        public void MakeMove(OnFieldMovement movement)
        {
            movement.Tetromino.CurrentPosition = new Vector2Int(movement.X, movement.Y);
            movement.Tetromino.CurrentRotation = movement.Rotation;
        }

        public bool IsPossibleMovement(OnFieldMovement movement)
        {
            Tetromino tetromino = movement.Tetromino;
            int rotation = movement.Rotation;
            int x = movement.X;
            int y = movement.Y;

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
                        CheckForNearBomb(i1,j1 + 1);
                    }
                }
            }
        }

        private bool InBounds(int x, int y)
        {
            return x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT;
        }

        private void CheckForNearBomb(int x, int y)
        {
            if (!InBounds(x, y))
            {
                return;
            }

            if (_gameField[x][y] == (int)SpotState.Bomb)
            {
                OnMomentForDetanateBomb = delegate { OnDetanateBomb(y); };
            }
        }

        private void OnDetanateBomb(int y)
        {
            OnMomentForDetanateBomb = delegate { };
            DeleteLine(y);
        }

        public void CreateBombLine(int y,int bombX, List<Tetromino> tetrominos)
        {
            MoveFieldUp(y);
            PlaceLineWithBomb(y, bombX, tetrominos);
        }

        private void MoveFieldUp(int y)
        {
            for (int j = 1; j <= y; j++)
            {
                for (int i = 0; i < WIDTH; i++)
                {
                    _gameField[i][j - 1] = _gameField[i][j];
                }
            }
        }

        private void PlaceLineWithBomb(int y, int bombX, List<Tetromino> tetrominos)
        {
            for (int i = 0; i < WIDTH; i++)
            {
                Tetromino oneBlockTetromino = tetrominos[i];
                oneBlockTetromino.CurrentRotation = 0;
                oneBlockTetromino.CurrentPosition = new Vector2Int(i, y);

                PlaceTetrimino(oneBlockTetromino);

                if (i == bombX)
                {
                    _gameField[i][y] = (int)SpotState.Bomb;
                }
            }
        }

        private void DeletePossibleLines()
        {
            for (int j = 0; j < HEIGHT; j++)
            {
                int i = 0;
                while (i < WIDTH)
                {
                    int spot = _gameField[i][j];
                    if (spot != (int)SpotState.Filled || spot == (int)SpotState.Bomb) 
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
