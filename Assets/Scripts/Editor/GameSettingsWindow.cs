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
        private GUISkin mGuiSkin;

        private GUIStyle mSelectedStyle;
        private GUIStyle mNormalStyle;

        private GameSettings mGameSettings;

        private int[][][] mTetrominoLayout;
        private Vector2Int[] mInitialPosition = new Vector2Int[Tetromino.BLOCK_ROTATIONS];
        private Color mColor;
        private string mName;

        private float mTimeToStep;
        private int mPointsByBreakingLine;
        private bool mControledRandomMode;

        private Vector2 mScrollImportedPosition;
        private Vector2 mScrollPosition;

        private int mCurrentEditing = -1;

        [MenuItem("Window/GameSettings Creator")]
        static void Init()
        {
            GameSettingsWindow window = (GameSettingsWindow)EditorWindow.GetWindow(typeof(GameSettingsWindow));
            window.Show();
        }

        private void OnEnable()
        {
            mCurrentEditing = -1;
            mTetrominoLayout = GetEmptyLayout();

            mGuiSkin = Resources.Load<GUISkin>("GameSettingsCreatorSkin");
            mSelectedStyle = mGuiSkin.GetStyle("Selected");
            mNormalStyle = mGuiSkin.GetStyle("Normal");
        }

        void OnGUI()
        {
            GUI.skin = mGuiSkin;
            mScrollPosition = GUILayout.BeginScrollView(mScrollPosition, true, true, GUILayout.Width(position.width), GUILayout.Height(position.height));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("IMPORT JSON", GUILayout.Width(150), GUILayout.Height(50)))
            {
                ImportJson();
            }

            if (GUILayout.Button("CREATE NEW", GUILayout.Width(150), GUILayout.Height(50)))
            {
                mCurrentEditing = -1;
                mGameSettings = new GameSettings();
                mGameSettings.Pieces = new List<TetrominoSpecs>();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (mGameSettings == null)
            {
                GUILayout.EndScrollView();
                return;
            }

            mTimeToStep = EditorGUILayout.FloatField("Time between update step", mTimeToStep);
            if (mTimeToStep < 0.01f) mTimeToStep = 0.01f;
            mPointsByBreakingLine = EditorGUILayout.IntField("Points by breaking lines", mPointsByBreakingLine);
            if (mPointsByBreakingLine < 0) mPointsByBreakingLine = 0;
            mControledRandomMode = EditorGUILayout.ToggleLeft("Controled random mode", mControledRandomMode);

            GUILayout.Space(20);
            var pieceHeight = 30f;
            var scrollHeight = Mathf.Clamp(pieceHeight * (2 + mGameSettings.Pieces.Count), 0, pieceHeight * 4);
            mScrollImportedPosition = GUILayout.BeginScrollView(
            mScrollImportedPosition, true, true,
                GUILayout.Width(position.width - 30),
            GUILayout.Height(scrollHeight));

            if (GUILayout.Button("ADD NEW", GUILayout.Width(position.width - 60), GUILayout.Height(pieceHeight)))
            {
                mScrollImportedPosition = new Vector2(0, scrollHeight);
                BeginEdit(-1, new TetrominoSpecs());
            }

            bool? mFinishedLayout = null;
            int counter = 0;
            for (int i = 0; i < mGameSettings.Pieces.Count; i++)
            {
                if (counter++ == 0)
                {
                    GUILayout.BeginHorizontal();
                    mFinishedLayout = false;
                }

                var index = i;
                GUI.color = mGameSettings.Pieces[i].Color;
                if (GUILayout.Button(mGameSettings.Pieces[i].Name, GUILayout.Width(150), GUILayout.Height(pieceHeight)))
                {
                    BeginEdit(index, mGameSettings.Pieces[i]);
                }

                if (counter == 4)
                {
                    GUILayout.EndHorizontal();
                    mFinishedLayout = true;
                    counter = 0;
                }
            }

            GUI.color = Color.white;

            if (mFinishedLayout.HasValue && !mFinishedLayout.Value)
                GUILayout.EndHorizontal();

            GUILayout.EndScrollView();

            if (mCurrentEditing != -1)
            {
                GUILayout.Space(30);
                mName = EditorGUILayout.TextField("Tetromino name", mName);
                GUILayout.Space(10);
                mColor = EditorGUILayout.ColorField("Tetromino Color", mColor);
                mColor.a = 1f;
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
                            bool active = mTetrominoLayout[j][i][l] == 1;
                            GUI.color = active ? mColor : Color.white;
                            if (GUILayout.Button(string.Format("{0},{1}", i, l), active ? mSelectedStyle : mNormalStyle))
                                mTetrominoLayout[j][i][l] = active ? 0 : 1;
                            GUI.color = Color.white;
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < mInitialPosition.Length; i++)
                {
                    mInitialPosition[i] = EditorGUILayout.Vector2IntField("Init Pos", mInitialPosition[i], GUILayout.Width(position.width / Tetromino.BLOCK_ROTATIONS - 10));
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
            mGameSettings.Pieces.RemoveAt(mCurrentEditing);
            mCurrentEditing = -1;
        }

        private void ImportJson()
        {
            var path = EditorUtility.OpenFilePanel("Choose GameSettings json.", "Assets", "json");
            if (path.Length != 0)
            {
                var json = File.ReadAllText(path);
                mGameSettings = JsonUtility.FromJson<GameSettings>(json);

                mTimeToStep = mGameSettings.TimeToStep;
                mPointsByBreakingLine = mGameSettings.PointsByBreakingLine;
                mControledRandomMode = mGameSettings.ControledRandomMode;
                
                mCurrentEditing = -1;
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
                mGameSettings.Pieces.Add(specs);
                index = mGameSettings.Pieces.Count - 1;
            }

            GUIUtility.keyboardControl = 0;
            GUIUtility.hotControl = 0;

            mCurrentEditing = index;

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

            mTetrominoLayout = blockPositions;
            mInitialPosition = specs.InitialPosition;
            mColor = specs.Color;
            mName = specs.Name;
        }

        private void EndEdit()
        {
            if (mGameSettings != null)
            {
                var specs = new TetrominoSpecs();
                specs.Name = mName;
                specs.Color = mColor;
                specs.InitialPosition = mInitialPosition;
                specs.SerializedBlockPositions = GetSerializableLayout(mTetrominoLayout);
                mGameSettings.Pieces[mCurrentEditing] = specs;
            }
        }

        private void ExportSettings()
        {
            var elements = 0;
            var squaredArea = Tetromino.BLOCK_AREA * Tetromino.BLOCK_AREA;
            foreach (var piece in mGameSettings.Pieces)
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
            mGameSettings.TimeToStep = mTimeToStep;
            mGameSettings.PointsByBreakingLine = mPointsByBreakingLine;
            mGameSettings.ControledRandomMode = mControledRandomMode;

            var json = JsonUtility.ToJson(mGameSettings, true);
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
