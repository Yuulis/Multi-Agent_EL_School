using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AgentInfo
{
    public int m_id;
    public TilemapPosition m_position;
    public bool m_active;
    public GameObject m_obj;

    public AgentInfo(int id, TilemapPosition tPos, bool active, GameObject obj)
    {
        m_id = id;
        m_position = tPos;
        m_active = active;
        m_obj = obj;
    }


    public void PrintAgentInfo()
    {
        Debug.Log($"Agent-{m_id} : " +
            $"[Position] tilemap = ({m_position.m_tilemap_pos.x}, {m_position.m_tilemap_pos.y}), " +
            $"fieldData = ({m_position.m_tilemapData_x}, {m_position.m_tilemapData_y}) " +
            $"[Active] = {m_active}");
    }
}
