using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FieldData {
    // Sprites list
    private readonly List<Sprite> m_tilemapSprites;

    // The data of the tile cell
    public List<TileCellInfo> m_data;


    public FieldData(Tilemap tilemap, int height, int width, List<Sprite> tilemapSprites)
    {
        m_tilemapSprites = tilemapSprites;
        m_data = new List<TileCellInfo>();
        for (int y = height / 2 - 1; y > -height / 2; y--)
        {
            for (int x = -width / 2; x < width / 2; x++)
            {
                TileCellInfo cellInfo = new();

                Vector3Int cellPosition = new(x, y, 0);
                cellInfo.position.m_tilemapPosition = cellPosition;

                if (tilemap.HasTile(cellPosition))
                {
                    cellInfo.tileNum = ChangeTileToTileNum(tilemap, cellPosition);
                }
                else
                {
                    cellInfo.tileNum = 0;
                }

                m_data.Add(cellInfo);
            }
        }
    }


    /// <summary>
    /// Sprite -> tilemapSprites / tileNum
    /// Empty    -> 0 / 1
    /// Exit     -> 1 / 2
    /// Obstacle -> 2 / 3
    /// Agent    -> 3 / 4
    /// (else)   -> - / 0
    /// </summary>
    /// <param name="tilemap">Tilemap</param>
    /// <param name="tilemapPosition">The position of the tilemap</param>
    /// <returns></returns>
    private int ChangeTileToTileNum(Tilemap tilemap, Vector3Int tilemapPosition) {
        if (tilemap.GetSprite(tilemapPosition) == m_tilemapSprites[0]) return 1;
        else if (tilemap.GetSprite(tilemapPosition) == m_tilemapSprites[1]) return 2;
        else if (tilemap.GetSprite(tilemapPosition) == m_tilemapSprites[2]) return 3;
        else return 0;
    }
}
