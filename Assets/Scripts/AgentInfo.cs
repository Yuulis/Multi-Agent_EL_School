using System;
using System.Collections.Generic;
using UnityEngine;

public class AgentInfo
{
    // Agent id
    public int m_id;

    // Agent's current floor 
    public int m_floorNum;

    // Agent's position on fieldDataIndex
    public Vector2Int m_positionIndex;

    // Is the agent active? (not reach goal)
    public bool m_active;

    // Agent's object
    public GameObject m_obj;

    // Agent's class
    public AgentControlByPoca m_agentControl;


    public AgentInfo(int id, int floorNum, Vector2Int position, bool active, GameObject obj, AgentControlByPoca agentControl)
    {
        m_id = id;
        m_floorNum = floorNum;
        m_positionIndex = position;
        m_active = active;
        m_obj = obj;
        m_agentControl = agentControl;
    }


    /// <summary>
    /// Output the agent's data. (for debug)
    /// </summary>
    public void PrintAgentInfo()
    {
        Debug.Log($"Agent-{m_id} : " +
            $"[Floor] = {m_floorNum}" +  
            $"[Position (fieldDataIndex)] = ({m_positionIndex.x}, {m_positionIndex.y}), " +
            $"[Active] = {m_active}");
    }
}
