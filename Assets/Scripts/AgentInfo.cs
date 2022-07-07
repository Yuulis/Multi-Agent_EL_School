using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AgentInfo
{
    // Agent id
    public int m_id;

    // Agent's tilemap-positon data
    public TilemapPositionInfo m_position;

    // Is the agent active? (not reach goal)
    public bool m_active;

    // Agent's object
    public GameObject m_obj;


    public AgentInfo(int id, TilemapPositionInfo tPos, bool active, GameObject obj)
    {
        m_id = id;
        m_position = tPos;
        m_active = active;
        m_obj = obj;
    }


    /// <summary>
    /// Output the agent's data. (for debug)
    /// </summary>
    public void PrintAgentInfo()
    {
        Debug.Log($"Agent_{m_id} : " +
            $"[Position] tilemap = ({m_position.m_tilemapPosition.x}, {m_position.m_tilemapPosition.y}), " +
            $"fieldData = ({m_position.m_fieldDataIndex_x}, {m_position.m_fieldDataIndex_y}) " +
            $"[Active] = {m_active}");
    }
}
