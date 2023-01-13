using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairInfo
{
    // All variable shows index
    public int floorFrom;
    public Vector2Int posFrom;
    public int floorTo;
    public Vector2Int posTo;


    public StairInfo(int floorFrom, Vector2Int posFrom, int floorTo, Vector2Int posTo)
    {
        this.floorFrom = floorFrom;
        this.posFrom = posFrom;
        this.floorTo = floorTo;
        this.posTo = posTo;
    }
}
