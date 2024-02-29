using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TetrisNetwork
{
    public class GameSettingsWindow : EditorWindow
    {
        private GUISkin _guiSkin;

        private GUIStyle _selectedStyle;
        private GUIStyle _normalStyle;

        private GameSettings _gameSettings;

        private int[][][] _tetrominoLayout;
        private Vector2Int[] _initialPosition = new Vector2Int[Tetromino.BLOCK_ROTATIONS];
        private Color _color;
        private string _name;

        private float _timeToStep;
        private int _pointsByBreakingLine;
        private bool _controledRandomMode;

        private Vector2 _scrollImportedPosition;
        private Vector2 _scrollPosition;

        private int _currentEditing = -1;

        [MenuItem("Window/GameSettings Creator")]
        static void Init()
        {
            GameSettingsWindow window = (GameSettingsWindow)EditorWindow.GetWindow(typeof(GameSettingsWindow));
            window.Show();
        }

        private void OnEnable()
        {
            _currentEditing = -1;
            _tetrominoLayout = GetEmptyLayout();

            _guiSkin = Resources.Load<GUISkin>("GameSettingsCreatorSkin");
            _selectedStyle = _guiSkin.GetStyle("Selected");
            _normalStyle = _guiSkin.GetStyle("Normal");
        }

        void OnGUI()
        {
            GUI.skin = _guiSkin;
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, true, true, GUILayout.Width(position.width), GUILayout.Height(position.height));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("IMPORT JSON", GUILayout.Width(150), GUILayout.Height(50)))
            {
                ImportJson();
            }

            if (GUILayout.Button("CREATE NEW", GUILayout.Width(150), GUILayout.Height(50)))
            {
                _currentEditing = -1;
                _gameSettings = new GameSettings();
                _gameSettings.Pieces = new List<TetrominoSpecs>();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (_gameSettings == null)
            {
                GUILayout.EndScrollView();
                return;
            }

            _timeToStep = EditorGUILayout.FloatField("Time between update step", _timeToStep);
            if (_timeToStep < 0.01f) _timeToStep = 0.01f;
            _pointsByBreakingLine = EditorGUILayout.IntField("Points by breaking lines", _pointsByBreakingLine);
            if (_pointsByBreakingLine < 0) _pointsByBreakingLine = 0;
            _controledRandomMode = EditorGUILayout.ToggleLeft("Controled random mode", _controledRandomMode);

            GUILayout.Space(20);
            var pieceHeight = 30f;
            var scrollHeight = Mathf.Clamp(pieceHeight * (2 + _gameSettings.Pieces.Count), 0, pieceHeight * 4);
            _scrollImportedPosition = GUILayout.BeginScrollView(
            _scrollImportedPosition, true, true,
                GUILayout.Width(position.width - 30),
            GUILayout.Height(scrollHeight));

            if (GUILayout.Button("ADD NEW", GUILayout.Width(position.width - 60), GUILayout.Height(pieceHeight)))
            {
                _scrollImportedPosition = new Vector2(0, scrollHeight);
                BeginEdit(-1, new TetrominoSpecs());
            }

            bool? finishedLayout = null;
            int counter = 0;
            for (int i = 0; i < _gameSettings.Pieces.Count; i++)
            {
                if (counter++ == 0)
                {
                    GUILayout.BeginHorizontal();
                    finishedLayout = false;
                }

                var index = i;
                GUI.color = _gameSettings.Pieces[i].Color;
                if (GUILayout.Button(_gameSettings.Pieces[i].Name, GUILayout.Width(150), GUILayout.Height(pieceHeight)))
                {
                    BeginEdit(index, _gameSettings.Pieces[i]);
                }

                if (counter == 4)
                {
                    GUILayout.EndHorizontal();
                    finishedLayout = true;
                    counter = 0;
                }
            }

            GUI.color = Color.white;

            if (finishedLayout.HasValue && !finishedLayout.Value)
                GUILayout.EndHorizontal();

            GUILayout.EndScrollView();

            if (_currentEditing != -1)
            {
                GUILayout.Space(30);
                _name = EditorGUILayout.TextField("Tetromino name", _name);
                GUILayout.Space(10);
                _color = EditorGUILayout.ColorField("Tetromino Color", _color);
                _color.a = 1f;
                GUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();
                for (int t = 0; t < Tetromino.BLOCK_ROTATIONS; t++)
                    GUILayout.TextArea("Rotation " + (t + 1));
                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < Tetromino.BLOCK_AREA; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int j = 0; j < Tetromino.BLOCK_ROTATIONS; j++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        for (int l = 0; l < Tetromino.BLOCK_AREA; l++)
                        {
                            bool active = _tetrominoLayout[j][i][l] == 1;
                            GUI.color = active ? _color : Color.white;
                            if (GUILayout.Button(string.Format("{0},{1}", i, l), active ? _selectedStyle : _normalStyle))
                                _tetrominoLayout[j][i][l] = active ? 0 : 1;
                            GUI.color = Color.white;
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < _initialPosition.Length; i++)
                {
                    _initialPosition[i] = EditorGUILayout.Vector2IntField("Init Pos", _initialPosition[i], GUILayout.Width(position.width / Tetromino.BLOCK_ROTATIONS - 10));
                }

                EditorGUILayout.EndHorizontal();

                EndEdit();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                GUI.color = Color.red;
                if (GUILayout.Button("REMOVE", GUILayout.Width(150), GUILayout.Height(50)))
                    RemoveSelected();
                GUI.color = Color.white;

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(50);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("SAVE", GUILayout.Width(150), GUILayout.Height(50)))
                ExportSettings();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();
        }

        private void RemoveSelected()
        {
            _gameSettings.Pieces.RemoveAt(_currentEditing);
            _currentEditing = -1;
        }

        private void ImportJson()
        {
            var path = EditorUtility.OpenFilePanel("Choose GameSettings json.", "Assets", "json");
            if (path.Length != 0)
            {
                var json = File.ReadAllText(path);
                _gameSettings = JsonUtility.FromJson<GameSettings>(json);

                _timeToStep = _gameSettings.TimeToStep;
                _pointsByBreakingLine = _gameSettings.PointsByBreakingLine;
                _controledRandomMode = _gameSettings.ControledRandomMode;
                
                _currentEditing = -1;
            }
        }

        private void BeginEdit(int index, TetrominoSpecs specs)
        {
            if (index == -1)
            {
                specs.Name = "New Tetromino";
                specs.Color = Color.white;
                specs.SerializedBlockPositions = GetSerializableLayout(GetEmptyLayout());
                specs.InitialPosition = GetInitialPositions();
                _gameSettings.Pieces.Add(specs);
                index = _gameSettings.Pieces.Count - 1;
            }

            GUIUtility.keyboardControl = 0;
            GUIUtility.hotControl = 0;

            _currentEditing = index;

            var pos = 0;
            var blockPositions = new int[Tetromino.BLOCK_ROTATIONS][][];
            for (int i = 0; i < blockPositions.Length; i++)
            {
                blockPositions[i] = new int[Tetromino.BLOCK_AREA][];
                for (int j = 0; j < blockPositions[i].Length; j++)
                {
                    blockPositions[i][j] = new int[Tetromino.BLOCK_AREA];
                    for (int k = 0; k < blockPositions[i][j].Length; k++)
                    {
                        blockPositions[i][j][k] = specs.SerializedBlockPositions[pos++];
                    }
                }
            }

            _tetrominoLayout = blockPositions;
            _initialPosition = specs.InitialPosition;
            _color = specs.Color;
            _name = specs.Name;
        }

        private void EndEdit()
        {
            if (_gameSettings != null)
            {
                var specs = new TetrominoSpecs();
                specs.Name = _name;
                specs.Color = _color;
                specs.InitialPosition = _initialPosition;
                specs.SerializedBlockPositions = GetSerializableLayout(_tetrominoLayout);
                _gameSettings.Pieces[_currentEditing] = specs;
            }
        }

        private void ExportSettings()
        {
            var elements = 0;
            var squaredArea = Tetromino.BLOCK_AREA * Tetromino.BLOCK_AREA;
            foreach (var piece in _gameSettings.Pieces)
            {
                elements = 0;
                while (elements * squaredArea < piece.SerializedBlockPositions.Count)
                {
                    if (piece.SerializedBlockPositions.Skip(elements * squaredArea).Take(squaredArea).Sum() == 0)
                    {
                        throw new System.Exception(
                            string.Format(
                                "Exportation failed. Rotation number {0} of piece {1} was left empty. A tetromino must have at least one block.",
                                elements + 1, piece.Name));
                    }

                    elements++;
                }
            }
            _gameSettings.TimeToStep = _timeToStep;
            _gameSettings.PointsByBreakingLine = _pointsByBreakingLine;
            _gameSettings.ControledRandomMode = _controledRandomMode;

            var json = JsonUtility.ToJson(_gameSettings, true);
            string path = EditorUtility.SaveFilePanel("Save GameSettings Json", "Assets", "GameSettings", "json");
            if (path.Length == 0)
            {
                Debug.LogError("Invalid path.");
                return;
            }

            File.WriteAllText(path, json);
        }

        private void CheckValidKey(KeyCode code, string keyTitle)
        {
            if (code == KeyCode.None)
                throw new System.Exception(string.Format("Exportation failed. {0} key cannot be None", keyTitle));
        }

        private Vector2Int[] GetInitialPositions()
        {
            var initialPositions = new Vector2Int[Tetromino.BLOCK_ROTATIONS];
            for (int i = 0; i < Tetromino.BLOCK_ROTATIONS; i++)
                initialPositions[i] = new Vector2Int(-Tetromino.BLOCK_AREA / 2, -Tetromino.BLOCK_AREA / 2);
            return initialPositions;
        }

        private int[][][] GetEmptyLayout()
        {
            var layout = new int[Tetromino.BLOCK_ROTATIONS][][];
            for (int i = 0; i < layout.Length; i++)
            {
                layout[i] = new int[Tetromino.BLOCK_AREA][];
                for (int j = 0; j < layout[i].Length; j++)
                {
                    layout[i][j] = new int[Tetromino.BLOCK_AREA];
                }
            }

            return layout;
        }

        private List<int> GetSerializableLayout(int[][][] layout)
        {
            var serializableLayout = new List<int>();
            for (int i = 0; i < Tetromino.BLOCK_ROTATIONS; i++)
            {
                for (int j = 0; j < Tetromino.BLOCK_AREA; j++)
                {
                    for (int l = 0; l < Tetromino.BLOCK_AREA; l++)
                    {
                        serializableLayout.Add(layout[i][j][l]);
                    }
                }
            }

            return serializableLayout;
        }
    }
}
