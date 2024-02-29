using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace TetrisNetwork
{
    public class GameController : CachedMonoBehaviour
    {
        [SerializeField] GameObject _tetrominoBlockPrefab;
        [SerializeField] Transform _tetrominoParent;

        [SerializeField] float timeToStep = 2f;

        //private GameSettings _gameSettings;
        private GameField _gameField;
        private List<TetrominoView> _tetrominos = new List<TetrominoView>();

        private float _timer = 0f;

        private TetrominoView _preview;
        private bool _refreshPreview;
        private bool _gameIsOver;

        private Pooling<TetrominoBlockView> _blockPool = new Pooling<TetrominoBlockView>();
        private Pooling<TetrominoView> _tetrominoPool = new Pooling<TetrominoView>();

        private Tetromino _currentTetromino
        {
            get
            {
                return (_tetrominos.Count > 0 && !_tetrominos[_tetrominos.Count - 1].IsLocked) ? _tetrominos[_tetrominos.Count - 1].CurrentTetromino : null;
            }
        }

        public void Start()
        {
            _blockPool.CreateMoreIfNeeded = true;
            _blockPool.Initialize(_tetrominoBlockPrefab, null);

            _tetrominoPool.CreateMoreIfNeeded = true;
            _tetrominoPool.Initialize(new GameObject("BlockHolder", typeof(RectTransform)), _tetrominoParent);
            _tetrominoPool.OnObjectCreationCallBack += x =>
            {
                x.OnDestroyTetrominoView = DestroyTetromino;
                x.BlockPool = _blockPool;
            };

            //TODO: Use Game Settings

            //Checks for the json file
            //var settingsFile = Resources.Load<TextAsset>(JSON_PATH);
            //if (settingsFile == null)
            //    throw new System.Exception(string.Format("GameSettings.json could not be found inside {0}. Create one in Window>GameSettings Creator.", JSON_PATH));

            //Loads the GameSettings Json
            ///var json = settingsFile.text;
            //mGameSettings = JsonUtility.FromJson<GameSettings>(json);
            //mGameSettings.CheckValidSettings();
            //timeToStep = mGameSettings.timeToStep;

            _gameField = new GameField();
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

        //Callback from Playfield to destroy a line in view
        private void DestroyLine(int y)
        {
            //GameScoreScreen.instance.AddPoints(mGameSettings.pointsByBreakingLine); //TODO: Use Game Settings
            GameScoreScreen.Instance.AddPoints(10);

            _tetrominos.ForEach(x => x.DestroyLine(y));
            _tetrominos.RemoveAll(x => x.Destroyed == true);
        }

        //Callback from Playfield to show game over in view
        private void SetGameOver()
        {
            _gameIsOver = true;
            GameOverScreen.Instance.ShowScreen();
        }

        //Call to the engine to create a new piece and create a representation of the random piece in view
        private void CreateTetromino()
        {
            if (_currentTetromino != null)
                _currentTetromino.IsLocked = true;

            var tetromino = _gameField.CreateTetromino();
            var tetrominoView = _tetrominoPool.Collect();
            tetrominoView.InitiateTetromino(tetromino);
            _tetrominos.Add(tetrominoView);

            if (_preview != null)
                _tetrominoPool.Release(_preview);

            _preview = _tetrominoPool.Collect();
            _preview.InitiateTetromino(tetromino, true);
            _refreshPreview = true;
        }

        //When all the blocks of a piece is destroyed, we must release ("destroy") it.
        private void DestroyTetromino(TetrominoView obj)
        {
            var index = _tetrominos.FindIndex(x => x == obj);
            _tetrominoPool.Release(obj);
            _tetrominos[index].Destroyed = true;
        }

        //Regular Unity Update method
        //Responsable for counting down and calling Step
        //Also responsable for gathering users input
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
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (_gameField.IsPossibleMovement(_currentTetromino.CurrentPosition.x,
                                                  _currentTetromino.CurrentPosition.y,
                                                  _currentTetromino,
                                                  _currentTetromino.NextRotation))
                {
                    _currentTetromino.CurrentRotation = _currentTetromino.NextRotation;
                    _refreshPreview = true;
                }
            }

            //Rotate Left
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (_gameField.IsPossibleMovement(_currentTetromino.CurrentPosition.x,
                                                  _currentTetromino.CurrentPosition.y,
                                                  _currentTetromino,
                                                  _currentTetromino.PreviousRotation))
                {
                    _currentTetromino.CurrentRotation = _currentTetromino.PreviousRotation;
                    _refreshPreview = true;
                }
            }

            //Move piece to the left
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (_gameField.IsPossibleMovement(_currentTetromino.CurrentPosition.x - 1,
                                                  _currentTetromino.CurrentPosition.y,
                                                  _currentTetromino,
                                                  _currentTetromino.CurrentRotation))
                {
                    _currentTetromino.CurrentPosition = new Vector2Int(_currentTetromino.CurrentPosition.x - 1, _currentTetromino.CurrentPosition.y);
                    _refreshPreview = true;
                }
            }

            //Move piece to the right
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (_gameField.IsPossibleMovement(_currentTetromino.CurrentPosition.x + 1,
                                                  _currentTetromino.CurrentPosition.y,
                                                  _currentTetromino,
                                                  _currentTetromino.CurrentRotation))
                {
                    _currentTetromino.CurrentPosition = new Vector2Int(_currentTetromino.CurrentPosition.x + 1, _currentTetromino.CurrentPosition.y);
                    _refreshPreview = true;
                }
            }

            //Make the piece fall faster
            //this is the only input with GetKey instead of GetKeyDown, because most of the time, users want to keep this button pressed and make the piece fall
            if (Input.GetKey(KeyCode.S))
            {
                if (_gameField.IsPossibleMovement(_currentTetromino.CurrentPosition.x,
                                                  _currentTetromino.CurrentPosition.y + 1,
                                                  _currentTetromino,
                                                  _currentTetromino.CurrentRotation))
                {
                    _currentTetromino.CurrentPosition = new Vector2Int(_currentTetromino.CurrentPosition.x, _currentTetromino.CurrentPosition.y + 1);
                }
            }

            //This part is responsable for rendering the preview piece in the right position
            if (_refreshPreview)
            {
                var y = _currentTetromino.CurrentPosition.y;
                while (_gameField.IsPossibleMovement(_currentTetromino.CurrentPosition.x,
                                                  y,
                                                  _currentTetromino,
                                                  _currentTetromino.CurrentRotation))
                {
                    y++;
                }

                _preview.ForcePosition(_currentTetromino.CurrentPosition.x, y - 1);
                _refreshPreview = false;
            }
        }
    }
}
