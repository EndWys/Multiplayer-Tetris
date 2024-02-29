using System.Collections;
using System.Collections.Generic;
using TetrisNetwork;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace TetrisNetwork
{

    public class TetrominoBlockView : PoolingObject
    {
        public override string ObjectName => "TetriminoBlock";

        public Vector2Int Position { get; private set; }

        private SpriteRenderer _spriteRenderer;

        //Gets references to the components
        public void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        //Sets the color of the block
        public void SetColor(Color c)
        {
            _spriteRenderer.color = c;
        }

        //Positioning the block
        public void MoveTo(int x, int y)
        {
            Position = new Vector2Int(x, y);
            CachedTransform.localPosition = new Vector3(x, -y, 0);
        }
    }
}