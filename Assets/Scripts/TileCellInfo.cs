using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileCellInfo
{
    // Tilemap-positon data
    public TilemapPositionInfo position;

    // Tile number
    public int tileNum;


    public TileCellInfo() 
    { 
        position = new TilemapPositionInfo();
        tileNum = 0;
    }
}
