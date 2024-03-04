using System.Collections;
using System.Collections.Generic;
using TetrisNetwork;
using UnityEngine;

public class FieldBackgroundBuilder
{
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
            for (int j = 4; j > -GameField.HEIGHT; j--)
            {
                var tile = _tilePool.Collect();
                tile.CachedTransform.localPosition = new Vector3(i, j, +2);
            }
        }
    }
}
