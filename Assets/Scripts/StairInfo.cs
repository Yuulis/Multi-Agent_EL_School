using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairInfo : MonoBehaviour
{
    // All variable is index
    // Floor1
    public int floorFrom;

    // pos1
    public Vector2Int posFrom;

    // floor2
    public int floorTo;

    // pos2
    public Vector2Int posTo;


    public StairInfo(int floorFrom, Vector2Int posFrom, int floorTo, Vector2Int posTo)
    {
        this.floorFrom = floorFrom;
        this.posFrom = posFrom;
        this.floorTo = floorTo;
        this.posTo = posTo;
    }
}
