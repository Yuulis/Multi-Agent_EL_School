using System;
using System.Collections.Generic;
using UnityEngine;

public class AgentInfoMultiFloor
{
    // Agent id
    public int id;

    // Agent's current floor 
    public int floorNum;

    // Agent's position on fieldDataIndex
    public Vector2Int positionIndex;

    // Is the agent active? (not reach goal)
    public bool active;

    // Whether agent used a stair at previous action or not
    public bool usedStair;

    // Agent's object
    public GameObject obj;

    // Agent's class
    public AgentControlMultiFloor agentControl;


    public AgentInfoMultiFloor(int id, int floorNum, Vector2Int positionIndex, bool active, bool usedStair, GameObject obj, AgentControlMultiFloor agentControl)
    {
        this.id = id;
        this.floorNum = floorNum;
        this.positionIndex = positionIndex;
        this.active = active;
        this.usedStair = usedStair;
        this.obj = obj;
        this.agentControl = agentControl;
    }


    /// <summary>
    /// Output the agent's data. (for debug)
    /// </summary>
    public void PrintAgentInfo()
    {
        Debug.Log($"Agent-{id} : " +
            $"[Floor] = {floorNum}" +
            $"[Position (fieldDataIndex)] = ({positionIndex.x}, {positionIndex.y}), " +
            $"[Active] = {active}");
    }
}
