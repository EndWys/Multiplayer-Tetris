using System.Collections;
using System.Collections.Generic;
using TetrisNetwork;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace TetrisNetwork
{
    public class TetrominoBlockView : PoolingObject
    {
        [SerializeField] Sprite _defaultSprite;
        [SerializeField] Sprite _bombSprite;
        public override string ObjectName => "TetriminoBlock";

        public Vector2Int Position { get; private set; }

        private SpriteRenderer _spriteRenderer;

        public void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public override void OnRelease()
        {
            base.OnRelease();
            SetBombsSpriteClientRpc();
        }

        public void SetColor(Color c)
        {
            SetColorClientRpc(c);
        }

        [ClientRpc]
        public void SetColorClientRpc(Color c)
        {
            _spriteRenderer.color = c;
        }

        public void MoveTo(int x, int y)
        {
            Position = new Vector2Int(x, y);
            CachedTransform.localPosition = new Vector3(x, -y, 0);
        }

        [ClientRpc]
        private void SetBlockSpriteClientRpc()
        {
            _spriteRenderer.sprite = _bombSprite;
        }

        public void SetBombSprite()
        {
            SetBlockSpriteClientRpc();
        }

        [ClientRpc]
        private void SetBombsSpriteClientRpc()
        {
            _spriteRenderer.sprite = _defaultSprite;
        }
    }
}