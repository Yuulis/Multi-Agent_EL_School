using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TilemapPositionInfo
{
    // The position on the tilemap
    public Vector3Int m_tilemapPosition;

    // Second-index of the fieldData2D
    public int m_fieldDataIndex_x;

    // First-index of the fieldData2D
    public int m_fieldDataIndex_y;


    public TilemapPositionInfo() 
    {
        m_tilemapPosition = new Vector3Int();
        m_fieldDataIndex_x = 0;
        m_fieldDataIndex_y = 0;
    }


    public (int, int) ChangeTCCStoFICS(Vector3Int tilemap_pos, int height, int width)
    {
        int i = height / 2 + tilemap_pos.y;
        if (height % 2 == 0) i--;

        int j = width / 2 + tilemap_pos.x;

        return (i, j);
    }


    public Vector3Int ChangeFICStoTCCS(int tilemapData_x, int tilemapData_y, int height, int width)
    {
        Vector3Int res = new();
        res.x = (width / 2) * -1 + tilemapData_x;

        res.y = (height / 2) * -1 + tilemapData_y;
        if (height % 2 == 0) res.y++;

        res.z = 0;

        return res;
    }
}


// tilemap-center coordinate system (TCCS)  -> tilemap
/// 
///    x-4 -3 -2 -1  0  1  2  3
///  y +------------------------
/// -3 | o  x  x  x  o  x  x  x
/// -2 | x  x  x  x  x  x  x  x
/// -1 | x  o  x  x  x  x  x  x
///  0 | x  o  x  x  c  x  x  o
///  1 | x  x  x  x  x  x  o  x
///  2 | x  x  x  o  x  x  x  o
///  3 | x  x  x  x  x  x  x  x  
///  4 | x  o  x  o  o  x  x  x   H = 8, W = 8
/// 

// fieldData-index coordinate system (FICS)  -> fieldData2D / observation
/// 
///    x 0  1  2  3  4  5  6  7
///  y +------------------------
///  0 | o  x  x  x  o  x  x  x
///  1 | x  x  x  x  x  x  x  x
///  2 | x  o  x  x  x  x  x  x
///  3 | x  o  x  x  c  x  x  o
///  4 | x  x  x  x  x  x  o  x
///  5 | x  x  x  o  x  x  x  o
///  6 | x  x  x  x  x  x  x  x  
///  7 | x  o  x  o  o  x  x  x   H = 8, W = 8
///

// TCCS -> FICS
///  fieldHeight = H, fieldWidth = W, TCCS-pos = (x, y, 0)
///  FICS-pos = (i, j)
///     i = H / 2 + y - 1 (H % 2 == 0)
///       = H / 2 + y     (H % 2 == 1)
///     j = W / 2 + x

// FICS -> TCCS
///  fieldHeight = H, fieldWidth = W, FICS-pos = (i, j)
///  TCCS-pos = (x, y, 0)
///     x = (W / 2) * -1 + j
///     y = (H / 2) * -1 + i + 1  (H % 2 == 0)
///       = (H / 2) * -1 + i      (H % 2 == 1)
