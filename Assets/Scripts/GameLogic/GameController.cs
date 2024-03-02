using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace TetrisNetwork
{
    public class GameController : NetworkBehaviour
    {
        private const string JSON_PATH = @"SupportFiles/GameSettings";

        [SerializeField] GameObject _tetrominoBlockPrefab;
        [SerializeField] GameObject _tetrominoPrefab;
        [SerializeField] Transform _tetrominoParent;

        [SerializeField] float timeToStep = 2f;

        [SerializeField] PlayerInputController _playerInput;

        private GameSettings _gameSettings;
        private GameField _gameField;
        private List<TetrominoView> _tetrominos = new List<TetrominoView>();

        private float _timer = 0f;

        private TetrominoView _preview;
        private bool _refreshPreview;
        private bool _gameIsOver;
        private bool _isConnected = false;

        private Pooling<TetrominoBlockView> _blockPool = new Pooling<TetrominoBlockView>();
        private Pooling<TetrominoView> _tetrominoPool = new Pooling<TetrominoView>();

        private Tetromino _currentTetromino { get; set; } = null;

        //int _clientId;
       
        public void StartGame(int clientId)
        {
            Debug.Log("Start Game");
            //_clientId = clientId;

            _playerInput.SetInputController(clientId);

            _playerInput.OnRotateRight = RotateTetrominoRight;
            _playerInput.OnRotateLeft = RotateTetrominoLeft;
            _playerInput.OnMoveLeft = MoveTetrominoLeft;
            _playerInput.OnMoveRight = MoveTetrominoRight;
            _playerInput.OnMoveDown = MoveTetrominoDown;

            _blockPool.CreateMoreIfNeeded = true;
            _blockPool.Initialize(_tetrominoBlockPrefab, null);

            _tetrominoPool.CreateMoreIfNeeded = true;
            _tetrominoPool.Initialize(_tetrominoPrefab, _tetrominoParent);
            _tetrominoPool.OnObjectCreationCallBack += x =>
            {
                x.OnDestroyTetrominoView = DestroyTetromino;
                x.BlockPool = _blockPool;
            };

            var settingsFile = Resources.Load<TextAsset>(JSON_PATH);
            if (settingsFile == null)
                throw new System.Exception(string.Format("GameSettings.json could not be found inside {0}. Create one in Window>GameSettings Creator.", JSON_PATH));

            var json = settingsFile.text;
            _gameSettings = JsonUtility.FromJson<GameSettings>(json);
            _gameSettings.CheckValidSettings();
            timeToStep = _gameSettings.TimeToStep;

            _gameField = new GameField(_gameSettings);
            _gameField.OnCurrentPieceReachBottom = CreateTetromino;
            _gameField.OnGameOver = SetGameOver;
            _gameField.OnDestroyLine = DestroyLine;

            GameOverScreen.Instance.HideScreen(0f);
            GameScoreScreen.Instance.HideScreen();

            RestartGame();

            _isConnected = true;
        }

        public void RestartGame()
        {
            GameOverScreen.Instance.HideScreen();
            GameScoreScreen.Instance.ResetScore();

            _gameIsOver = false;
            _timer = 0f;

            _gameField.Restart();
            _tetrominoPool.ReleaseAll();
            _tetrominos.Clear();

            CreateTetromino();
        }

        private void DestroyLine(int y)
        {
            GameScoreScreen.Instance.AddPoints(_gameSettings.PointsByBreakingLine);

            _tetrominos.ForEach(x => x.DestroyLine(y));
            _tetrominos.RemoveAll(x => x.Destroyed == true);
        }

        private void SetGameOver()
        {
            _gameIsOver = true;
            GameOverScreen.Instance.ShowScreen();
        }

        private void CreateTetromino()
        {
            if (_currentTetromino != null)
                _currentTetromino.IsLocked = true;

            var tetromino = _gameField.CreateTetromino();
            var tetrominoView = _tetrominoPool.Collect();
            tetrominoView.InitiateTetromino(tetromino);
            _tetrominos.Add(tetrominoView);

            _currentTetromino = tetromino;

            if (_preview != null)
                _tetrominoPool.Release(_preview);

            _preview = _tetrominoPool.Collect();
            _preview.InitiateTetromino(tetromino, true);
            _refreshPreview = true;
        }

        private void DestroyTetromino(TetrominoView obj)
        {
            var index = _tetrominos.FindIndex(x => x == obj);
            _tetrominoPool.Release(obj);
            _tetrominos[index].Destroyed = true;
        }


        public void Update()
        {
            if (!_isConnected) return;

            if (_gameIsOver) return;

            _timer += Time.deltaTime;
            if (_timer > timeToStep)
            {
                _timer = 0;
                _gameField.Step();
            }

            if (_currentTetromino == null) return;

            if (_refreshPreview)
            {
                var move = new OnFieldMovement(_currentTetromino,_currentTetromino.CurrentRotation,
                    _currentTetromino.CurrentPosition.x,_currentTetromino.CurrentPosition.y);

                while (_gameField.IsPossibleMovement(move))
                {
                    move.Y++;
                }

                _preview.ForcePosition(move.X, move.Y - 1);
                _refreshPreview = false;
            }
        }

        void RotateTetrominoRight()
        {
            if (_gameIsOver || _currentTetromino == null) return;

            var move = new OnFieldMovement(_currentTetromino, _currentTetromino.NextRotation,
                _currentTetromino.CurrentPosition.x, _currentTetromino.CurrentPosition.y);

            if (_gameField.IsPossibleMovement(move))
            {
                _gameField.MakeMove(move);
                _refreshPreview = true;
            }
        }

        void RotateTetrominoLeft()
        {
            if (_gameIsOver || _currentTetromino == null) return;

            var move = new OnFieldMovement(_currentTetromino, _currentTetromino.PreviousRotation,
                _currentTetromino.CurrentPosition.x, _currentTetromino.CurrentPosition.y);

            if (_gameField.IsPossibleMovement(move))
            {
                _gameField.MakeMove(move);
                _refreshPreview = true;
            }
        }

        void MoveTetrominoRight()
        {
            if (_gameIsOver || _currentTetromino == null) return;

            var move = new OnFieldMovement(_currentTetromino, _currentTetromino.CurrentRotation,
                _currentTetromino.CurrentPosition.x + 1, _currentTetromino.CurrentPosition.y);

            if (_gameField.IsPossibleMovement(move))
            {
                _gameField.MakeMove(move);
                _refreshPreview = true;
            }
        }

        void MoveTetrominoLeft()
        {
            if (_gameIsOver || _currentTetromino == null) return;

            var move = new OnFieldMovement(_currentTetromino, _currentTetromino.CurrentRotation,
                _currentTetromino.CurrentPosition.x - 1, _currentTetromino.CurrentPosition.y);

            if (_gameField.IsPossibleMovement(move))
            {
                _gameField.MakeMove(move);
                _refreshPreview = true;
            }
        }

        void MoveTetrominoDown()
        {
            if (_gameIsOver || _currentTetromino == null) return;

            var move = new OnFieldMovement(_currentTetromino, _currentTetromino.CurrentRotation,
                                _currentTetromino.CurrentPosition.x, _currentTetromino.CurrentPosition.y + 1);

            if (_gameField.IsPossibleMovement(move))
            {
                _gameField.MakeMove(move);
            }
        }
    }
}
