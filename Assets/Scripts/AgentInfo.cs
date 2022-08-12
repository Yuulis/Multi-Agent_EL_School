using System;
using System.Collections.Generic;
using UnityEngine;

public class AgentInfo
{
    // Agent id
    public int m_id;

    // Agent's position on fieldDataIndex
    public Vector2Int m_positionIndex;

    // Is the agent active? (not reach goal)
    public bool m_active;

    // Agent's object
    public GameObject m_obj;


    public AgentInfo(int id, Vector2Int position, bool active, GameObject obj)
    {
        m_id = id;
        m_positionIndex = position;
        m_active = active;
        m_obj = obj;
    }


    /// <summary>
    /// Output the agent's data. (for debug)
    /// </summary>
    public void PrintAgentInfo()
    {
        Debug.Log($"Agent-{m_id} : " +
            $"[Position (fieldDataIndex)]  = ({m_positionIndex.x}, {m_positionIndex.y}), " +
            $"[Active] = {m_active}");
    }
}
