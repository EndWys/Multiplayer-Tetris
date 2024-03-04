using Unity.Netcode;
using UnityEngine;

namespace TetrisNetwork
{
    public class BombView : TetrominoView
    {
        public override void InitiateTetromino(Tetromino tetrimino, bool isPreview = false)
        {
            base.InitiateTetromino(tetrimino, isPreview);
            SetBombSpriteForBlocks();
        }
        private void SetBombSpriteForBlocks()
        {
            foreach (var piece in _pieces)
            {
                piece.SetBombSprite();
            }
        }
    }
}
