using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellInfo : MonoBehaviour
{
    /*
     * === cell-type ===
     * 0 : Unable to spawn
     * 1 : Empty
     * 2 : Exit
     * 3 : Obstacle
     * 4 : Upstair
     * 5 : Downstair
     */
    int cellType;

    // If cell-type is 4 or 5, this is not null. 
    StairInfo stairInfo;


    public CellInfo(int cellType, StairInfo stairInfo)
    {
        this.cellType = cellType;
        this.stairInfo = stairInfo;
    }

}
