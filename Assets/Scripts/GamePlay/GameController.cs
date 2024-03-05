using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace TetrisNetwork
{
    public class GameController : MonoBehaviour
    {
        private const string JSON_PATH = @"SupportFiles/GameSettings";

        [SerializeField] GameObject _tetrominoBlockPrefab;
        [SerializeField] GameObject _tetrominoPrefab;
        [SerializeField] GameObject _bombPrefab;
        [SerializeField] GameObject _backgroundTile;
        [SerializeField] Transform _tetrominoParent;

        [SerializeField] float timeToStep = 2f;

        [SerializeField] PlayerInputConnenctor _inputConnector;

        private FieldBackgroundBuilder _fieldBackground;

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
        private Pooling<BombView> _bombPool = new Pooling<BombView>();

        private Tetromino _currentTetromino { get; set; } = null;

        private LocalMatchStarter _matchController;

        private int _clientId;
        public int ClientId => _clientId;

        [Inject]
        public void Construct(LocalMatchStarter matchStarter)
        {
            _matchController = matchStarter;
        }

        public void StartGame(int clientId)
        {
            AudioController.PlayMusic(AudioController.Music._gameMusic);

            _clientId = clientId;

            ConnectInput();

            InitializePools();

            SetGameSettings();

            InitializeGameField();

            RestartGame();

            _isConnected = true;
        }

        private void InitializePools()
        {
            _blockPool.CreateMoreIfNeeded = true;
            _blockPool.Initialize(_tetrominoBlockPrefab, null);

            _tetrominoPool.CreateMoreIfNeeded = true;
            _tetrominoPool.Initialize(_tetrominoPrefab, _tetrominoParent);
            _tetrominoPool.OnObjectCreationCallBack += x =>
            {
                x.OnDestroyTetrominoView = DestroyTetromino;
                x.BlockPool = _blockPool;
            };

            _bombPool.CreateMoreIfNeeded = true;
            _bombPool.Initialize(_bombPrefab, _tetrominoParent);
            _bombPool.OnObjectCreationCallBack += x =>
            {
                x.OnDestroyTetrominoView = DestroyTetromino;
                x.BlockPool = _blockPool;
            };
        }

        private void SetGameSettings()
        {

            var settingsFile = Resources.Load<TextAsset>(JSON_PATH);
            if (settingsFile == null)
                throw new System.Exception(string.Format("GameSettings.json could not be found inside {0}. Create one in Window>GameSettings Creator.", JSON_PATH));

            var json = settingsFile.text;
            _gameSettings = JsonUtility.FromJson<GameSettings>(json);
            _gameSettings.CheckValidSettings();
            timeToStep = _gameSettings.TimeToStep;
        }

        private void InitializeGameField()
        {
            _fieldBackground = new FieldBackgroundBuilder(_backgroundTile, _tetrominoParent);
            _fieldBackground.BuildFieldBackground();

            _gameField = new GameField(_gameSettings);
            _gameField.OnCurrentPieceReachBottom = CreateTetromino;
            _gameField.OnGameOver = OnGameOver;
            _gameField.OnDestroyLine = DestroyLine;
        }

        public void RestartGame()
        {
            _gameIsOver = false;
            _timer = 0f;

            _gameField.Restart();
            _tetrominoPool.ReleaseAll();
            _bombPool.ReleaseAll();
            _tetrominos.Clear();

            CreateTetromino();

            _matchController.ShowYoursGameField(_clientId);
        }

        private void DestroyLine(int y)
        {
            _tetrominos.ForEach(x => x.DestroyLine(y));
            _tetrominos.RemoveAll(x => x.Destroyed == true);

            _matchController.OnDestroyLine(_gameSettings.PointsByBreakingLine, _clientId);
        }

        public void WaitMomentToCreateBombLine(int y)
        {
            _gameField.OnMomentToCreateLine = delegate { CreateBombLine(y); };
        }

        public void CreateBombLine(int y)
        {
            _gameField.OnMomentToCreateLine = delegate { };

            _tetrominos.ForEach(x => x.OnCreateNewLine(y));

            int bombX = RandomGenerator.random.Next(0, GameField.WIDTH);
            var tetrominoLine = CreateLineOfOneBlockTetrominos(bombX);

            _gameField.CreateBombLine(y, bombX, tetrominoLine);

            _refreshPreview = true;
        }

        private void OnGameOver()
        {
            _matchController.OnGameOver(_clientId);
        }

        public void SetGameOver()
        {
            _gameIsOver = true;
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

        private List<Tetromino> CreateLineOfOneBlockTetrominos(int bombX)
        {
            List<Tetromino> line = new List<Tetromino>();

            for(int i = 0; i < GameField.WIDTH; i++)
            {
                line.Add(CreateOneBlockTetromino(i == bombX));
            }

            return line;

        }

        private Tetromino CreateOneBlockTetromino(bool isBomb = false)
        {
            Tetromino tetromino = _gameField.CreateOneBlockTetromino();
            TetrominoView tetrominoView;

            if (isBomb)
            {
                tetrominoView = _bombPool.Collect();
            }
            else
            {
                tetrominoView = _tetrominoPool.Collect();
            }
            
            tetrominoView.InitiateTetromino(tetromino);
            _tetrominos.Add(tetrominoView);

            return tetromino;
        }

        private void DestroyTetromino(TetrominoView obj)
        {
            var index = _tetrominos.FindIndex(x => x == obj);
            _tetrominoPool.Release(obj);
            _tetrominos[index].Destroyed = true;
        }

        private void ConnectInput()
        {
            _inputConnector.SetClientId(_clientId);
            _inputConnector.ConnectSignal();

            _inputConnector.ConnectAction(InputT.RotateRight, RotateTetrominoRight);
            _inputConnector.ConnectAction(InputT.RotateLeft, RotateTetrominoLeft);
            _inputConnector.ConnectAction(InputT.MoveLeft, MoveTetrominoLeft);
            _inputConnector.ConnectAction(InputT.MoveRight, MoveTetrominoRight);
            _inputConnector.ConnectAction(InputT.MoveDown, MoveTetrominoDown);
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
