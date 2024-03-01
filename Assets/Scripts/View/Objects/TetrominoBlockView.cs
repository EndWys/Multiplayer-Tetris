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
        public override string ObjectName => "TetriminoBlock";

        public Vector2Int Position { get; private set; }

        private SpriteRenderer _spriteRenderer;

        public void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetColor(Color c)
        {
            _spriteRenderer.color = c;
        }

        public void MoveTo(int x, int y)
        {
            Position = new Vector2Int(x, y);
            CachedTransform.localPosition = new Vector3(x, -y, 0);
        }

        public override void Spawn()
        {
            SpawnBlockServerRpc();
        }

        [ServerRpc]
        private void SpawnBlockServerRpc()
        {
            Debug.Log("TryToSpawn");

            var o = GetComponent<NetworkObject>();

            o.Spawn();
        }
    }
}