using UnityEngine;

namespace TetrisNetwork {
    public class FieldBackgroundBuilder
    {
        const int BACKGROUND_SIZE_OFFSET = 4;

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
                for (int j = BACKGROUND_SIZE_OFFSET; j > -GameField.HEIGHT; j--)
                {
                    var tile = _tilePool.Collect();
                    tile.CachedTransform.localPosition = new Vector3(i, j, +2);
                }
            }
        }
    }
}
