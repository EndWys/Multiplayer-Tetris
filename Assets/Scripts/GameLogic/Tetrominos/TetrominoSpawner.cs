using System;
using System.Collections.Generic;

namespace TetrisNetwork
{
    public class TetrominoSpawner
    {
        private bool _controledRandom;

        private List<TetrominoSpecs> _allTetriminos = new List<TetrominoSpecs>();
        private List<TetrominoSpecs> _availableTetriminos = new List<TetrominoSpecs>();

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

        private List<TetrominoSpecs> GetFullTetrominoBaseList()
        {
            var allTetriminos = new List<TetrominoSpecs>(_allTetriminos);
            return allTetriminos;
        }
    }
}
