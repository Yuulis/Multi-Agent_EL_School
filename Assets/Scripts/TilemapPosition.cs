using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TilemapPosition
{
    public Vector3Int m_tilemap_pos; // Position on the tilemap (TCCS)
    public int m_tilemapData_x;  // First-index of the tilemap data (FICS)
    public int m_tilemapData_y;  // Second-index of the tilemap data (FICS)

    //TilemapPosition(Vector3Int tilemap_pos, int tilemapData_x, int tilemapData_y) {
    //    m_tilemap_pos = tilemap_pos;
    //    m_tilemapData_x = tilemapData_x;
    //    m_tilemapData_y = tilemapData_y;
    //}


    public (int, int) ChangeTCCStoFICS(Vector3Int tilemap_pos, int height, int width)
    {
        int i = width / 2 + tilemap_pos.x;
        int j = height / 2 + tilemap_pos.y;
        return (i, j);
    }


    public Vector3Int ChangeFICStoTCCS(int tilemapData_x, int tilemapData_y, int height, int width)
    {
        Vector3Int res = new();
        res.x = (width / 2) * -1 + tilemapData_x;
        res.y = (height / 2 - tilemapData_y) * -1;
        res.z = 0;
        return res;
    }
}


// tilemap-center coordinate system (TCCS)  -> tilemap
/// 
///    x-3 -2 -1  0  1  2  3
///  y +---------------------
/// -3 | o  x  x  x  o  x  x
/// -2 | x  x  x  x  x  x  x
/// -1 | x  o  x  x  x  x  x
///  0 | x  o  x  c  x  x  x
///  1 | x  x  x  x  x  x  o
///  2 | x  x  x  o  x  x  x
///  3 | x  x  x  x  x  x  x   H = 7, W = 7
/// 

// fieldData-index coordinate system (FICS)  -> fieldData / observation
/// 
///    j 0  1  2  3  4  5  6
///  i +---------------------
///  0 | c  x  x  x  o  x  x
///  1 | x  x  x  x  x  x  x
///  2 | x  o  x  x  x  x  x
///  3 | x  o  x  x  x  x  x
///  4 | x  x  x  x  x  x  o
///  5 | x  x  x  o  x  x  x
///  6 | x  x  x  x  x  x  x   H = 7, W = 7
///

// TCCS -> FICS
///  fieldHeight = H, fieldWidth = W, TCCS-pos = (x, y, 0)
///  FICS-pos = (i, j)
///     i = W / 2 + x
///     j = H / 2 + y

// FICS -> TCCS
///  fieldHeight = H, fieldWidth = W, FICS-pos = (i, j)
///  TCCS-pos = (x, y, 0)
///     x = (W / 2) * -1 + j
///     y = (H / 2 - i) * -1
