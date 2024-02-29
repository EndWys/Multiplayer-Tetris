using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TetrisNetwork
{
    public class GameController : CachedMonoBehaviour
    {
        private const string JSON_PATH = @"SupportFiles/GameSettings";

        [SerializeField] GameObject _tetrominoBlockPrefab;
        [SerializeField] Transform _tetrominoParent;

        [SerializeField] float timeToStep = 2f;

        private GameSettings _gameSettings;
        private PlayerInput _playerInput;
        private GameField _gameField;
        private List<TetrominoView> _tetrominos = new List<TetrominoView>();

        private float _timer = 0f;

        private TetrominoView _preview;
        private bool _refreshPreview;
        private bool _gameIsOver;

        private Pooling<TetrominoBlockView> _blockPool = new Pooling<TetrominoBlockView>();
        private Pooling<TetrominoView> _tetrominoPool = new Pooling<TetrominoView>();

        private Tetromino _currentTetromino { get; set; } = null;

        public void Start()
        {
            _playerInput = new PlayerInput();

#if UNITY_EDITOR
            _playerInput.SetInputController(new EditorInputController());
#else
            _playerInput.SetInputController(new EditorInputController());
#endif

            _blockPool.CreateMoreIfNeeded = true;
            _blockPool.Initialize(_tetrominoBlockPrefab, null);

            _tetrominoPool.CreateMoreIfNeeded = true;
            _tetrominoPool.Initialize(new GameObject("BlockHolder", typeof(RectTransform)), _tetrominoParent);
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
            if (_gameIsOver) return;

            _timer += Time.deltaTime;
            if (_timer > timeToStep)
            {
                _timer = 0;
                _gameField.Step();
            }

            if (_currentTetromino == null) return;

            //Rotate Right
            if (_playerInput.MakeRotateRight())
            {
                if (_gameField.IsPossibleMovement(_currentTetromino,
                                                  _currentTetromino.NextRotation,
                                                  _currentTetromino.CurrentPosition.x,
                                                  _currentTetromino.CurrentPosition.y))
                {
                    _currentTetromino.CurrentRotation = _currentTetromino.NextRotation;
                    _refreshPreview = true;
                }
            }

            //Rotate Left
            if (_playerInput.MakeRotateLeft())
            {
                if (_gameField.IsPossibleMovement(_currentTetromino,
                                                  _currentTetromino.PreviousRotation,
                                                  _currentTetromino.CurrentPosition.x,
                                                  _currentTetromino.CurrentPosition.y))
                {
                    _currentTetromino.CurrentRotation = _currentTetromino.PreviousRotation;
                    _refreshPreview = true;
                }
            }

            //Move piece to the left
            if (_playerInput.MakeMoveLeft())
            {
                if (_gameField.IsPossibleMovement(_currentTetromino,
                                                  _currentTetromino.CurrentRotation,
                                                  _currentTetromino.CurrentPosition.x - 1,
                                                  _currentTetromino.CurrentPosition.y))
                {
                    _currentTetromino.CurrentPosition = new Vector2Int(_currentTetromino.CurrentPosition.x - 1, _currentTetromino.CurrentPosition.y);
                    _refreshPreview = true;
                }
            }

            //Move piece to the right
            if (_playerInput.MakeMoveRight())
            {
                if (_gameField.IsPossibleMovement(_currentTetromino,
                                                  _currentTetromino.CurrentRotation,
                                                  _currentTetromino.CurrentPosition.x + 1,
                                                  _currentTetromino.CurrentPosition.y))
                {
                    _currentTetromino.CurrentPosition = new Vector2Int(_currentTetromino.CurrentPosition.x + 1, _currentTetromino.CurrentPosition.y);
                    _refreshPreview = true;
                }
            }

            //Make the piece fall faster
            //this is the only input with GetKey instead of GetKeyDown, because most of the time, users want to keep this button pressed and make the piece fall
            if (_playerInput.MakeMoveDown())
            {
                if (_gameField.IsPossibleMovement(_currentTetromino,
                                                  _currentTetromino.CurrentRotation,
                                                  _currentTetromino.CurrentPosition.x,
                                                  _currentTetromino.CurrentPosition.y + 1))
                {
                    _currentTetromino.CurrentPosition = new Vector2Int(_currentTetromino.CurrentPosition.x, _currentTetromino.CurrentPosition.y + 1);
                }
            }

            //This part is responsable for rendering the preview piece in the right position
            if (_refreshPreview)
            {
                var y = _currentTetromino.CurrentPosition.y;
                while (_gameField.IsPossibleMovement(_currentTetromino,
                                                  _currentTetromino.CurrentRotation,
                                                  _currentTetromino.CurrentPosition.x,
                                                  y))
                {
                    y++;
                }

                _preview.ForcePosition(_currentTetromino.CurrentPosition.x, y - 1);
                _refreshPreview = false;
            }
        }
    }
}
