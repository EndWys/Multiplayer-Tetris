using System;
using System.Collections.Generic;
using UnityEngine;

namespace TetrisNetwork
{
    public class TetrominoView : PoolingObject
    {
        public override string ObjectName => "TetriminoView";

        public bool IsLocked => CurrentTetromino.IsLocked;

        public bool Destroyed;
        public Tetromino CurrentTetromino;
        public Action<TetrominoView> OnDestroyTetrominoView;
        public Pooling<TetrominoBlockView> BlockPool;

        private readonly List<TetrominoBlockView> _pieces = new List<TetrominoBlockView>();
        private Color _blockColor;
        private RectTransform _rectTransform;

        private Vector2Int _tetriminoPosition;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public override void OnCollect()
        {
            base.OnCollect();

            Destroyed = false;

            _rectTransform.anchorMin = Vector2.zero;
            _rectTransform.anchorMax = Vector2.one;
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
        }

        public void InitiateTetromino(Tetromino tetrimino, bool isPreview = false)
        {
            var ghostColor = new Color(1, 1, 1, 0.5f);

            CurrentTetromino = tetrimino;

            if (!isPreview)
                CurrentTetromino.OnChangePosition = ChangePosition;
            else
                _rectTransform.SetAsFirstSibling();

            CurrentTetromino.OnChangeRotation += Draw;

            _blockColor = (isPreview) ? ghostColor : CurrentTetromino.Color;
            _pieces.ForEach(x => x.SetColor(_blockColor));

            ChangePosition();
            Draw();
        }

        public void DestroyLine(int y)
        {
            for (int i = 0; i < _pieces.Count; i++)
            {
                if (_pieces[i].Position.y.Equals(y))
                {
                    BlockPool.Release(_pieces[i]);
                    _pieces[i] = null;
                    continue;
                }

                if (_pieces[i].Position.y <= y)
                    MovePiece(_pieces[i], _pieces[i].Position.x, _pieces[i].Position.y + 1);
            }

            _pieces.RemoveAll(x => x == null);

            if (_pieces.Count == 0)
                OnDestroyTetrominoView.Invoke(this);
        }

        public void CreateNewLine(int y)
        {
            for (int i = 0; i < _pieces.Count; i++)
            {
                if (_pieces[i].Position.y <= y)
                {
                    MovePiece(_pieces[i], _pieces[i].Position.x, _pieces[i].Position.y - 1);
                }
            }
        }

        public void ForcePosition(int x, int y)
        {
            _tetriminoPosition = new Vector2Int(x, y);
            Draw();
        }

        private void ChangePosition()
        {
            _tetriminoPosition = CurrentTetromino.CurrentPosition;
            Draw();
        }

        private void Draw()
        {
            var cRot = CurrentTetromino.BlockPositions[CurrentTetromino.CurrentRotation];
            var currentIndex = 0;

            for (int i = 0; i < cRot.Length; i++)
            {
                for (int j = 0; j < cRot[i].Length; j++)
                {
                    if (cRot[i][j] == 0) continue;

                    var piece = _pieces.Count > currentIndex ? _pieces[currentIndex] : null;
                    if (piece == null)
                    {
                        piece = BlockPool.Collect(transform);
                        piece.SetColor(_blockColor);
                        _pieces.Add(piece);
                    }

                    currentIndex++;
                    MovePiece(piece, _tetriminoPosition.x + j, _tetriminoPosition.y + i);
                }
            }
        }

        private void MovePiece(TetrominoBlockView block, int x, int y)
        {
            block.MoveTo(x, y);
        }
    }
}
