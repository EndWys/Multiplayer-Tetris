using UnityEngine;

namespace TetrisNetwork {
    public class FieldBackgroundBuilder
    {
        private const int BACKGROUND_LOWEST_POINT_GLOBAL = -GameField.HEIGHT;
        private const int BACKGROUND_HIGH_OFFSET = 4;
        private const int BACKGROUND_TILE_DEPTH = 2;

        private Pooling<BackgroundTileView> _tilePool = new Pooling<BackgroundTileView>();

        public FieldBackgroundBuilder(GameObject tilePrefab, Transform parent)
        {
            _tilePool.CreateMoreIfNeeded = true;
            _tilePool.Initialize(tilePrefab, parent);
        }

        public void BuildFieldBackground()
        {
            for (int i = 0; i < GameField.WIDTH; i++)
            {
                for (int j = BACKGROUND_HIGH_OFFSET; j > BACKGROUND_LOWEST_POINT_GLOBAL; j--)
                {
                    var tile = _tilePool.Collect();
                    tile.CachedTransform.localPosition = new Vector3(i, j, BACKGROUND_TILE_DEPTH);
                }
            }
        }
    }
}
