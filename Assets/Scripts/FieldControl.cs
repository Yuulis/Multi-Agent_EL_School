using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Tilemaps;

public class FieldControl : MonoBehaviour
{
    // Settings
    public GameObject Settings_obj;
    Settings settings;

    // FieldData
    [HideInInspector] public FieldData fieldData;
    [HideInInspector] public List<List<TileCellInfo>> fieldData2D;

    // Tilemaps resources
    public List<Sprite> tilemapSprites;
    public Tilemap field_tilemap;
    public Tilemap agent_tilemap;
    public TileBase agent_tile;

    // Agents resources
    [HideInInspector] public int activeAgentsNum;
    public List<GameObject> agentsList;
    public List<AgentInfo> agentsData;


    private void Start()
    {
        settings = Settings_obj.GetComponent<Settings>();
        activeAgentsNum = settings.agentCnt;

        ResetFieldData(settings.fieldHeight, settings.fieldWidth);
        InitializeField();

        // For debug
        if (settings.debugMode) PrintFieldData(settings.fieldHeight, settings.fieldWidth);
    }


    /// <summary>
    /// Initializing Field data.
    /// </summary>
    public void InitializeField()
    {
        agent_tilemap.ClearAllTiles();
        RandomSetAgent(settings.fieldHeight, settings.fieldWidth, settings.agentCnt);
    }


    /// <summary>
    /// Reset Field data.
    /// </summary>
    /// <param name="width">Field's width</param>
    private void ResetFieldData(int height, int width)
    {
        fieldData = new(field_tilemap, height, width, tilemapSprites);
        fieldData2D = new();

        for (int y = 0; y < height; y++)
        {
            List<TileCellInfo> list = new();
            for (int x = 0; x < width; x++)
            {
                fieldData.m_data[y * width + x].position.m_fieldDataIndex_x = x;
                fieldData.m_data[y * width + x].position.m_fieldDataIndex_y = y;
                list.Add(fieldData.m_data[y * width + x]);
            }
            fieldData2D.Add(list);
        }
    }


    /// <summary>
    /// Agents set random place in field.
    /// </summary>
    /// <param name="height">Field's height</param>
    /// <param name="width">Field's width</param>
    /// <param name="agentNum">Number of Agents</param>
    private void RandomSetAgent(int height, int width, int agentNum)
    {
        agentsData = new();

        int cnt = 0;
        while (cnt < agentNum)
        {
            TilemapPositionInfo spawn_tPos = new()
            {
                m_fieldDataIndex_x = Random.Range(0, width),
                m_fieldDataIndex_y = Random.Range(0, height)
            };
            spawn_tPos.m_tilemapPosition = spawn_tPos.ChangeFICStoTCCS(
                spawn_tPos.m_fieldDataIndex_x, 
                spawn_tPos.m_fieldDataIndex_y, 
                height, 
                width
            );

            // Only Empty position
            if (field_tilemap.GetSprite(spawn_tPos.m_tilemapPosition) == tilemapSprites[0] && agent_tilemap.GetTile(spawn_tPos.m_tilemapPosition) != agent_tile)
            {
                agent_tilemap.SetTile(spawn_tPos.m_tilemapPosition, agent_tile);

                TileCellInfo cellInfo = new();
                cellInfo.position = spawn_tPos;
                cellInfo.tileNum = 10 + cnt;
                fieldData2D[spawn_tPos.m_fieldDataIndex_y][spawn_tPos.m_fieldDataIndex_x] = cellInfo;

                AgentInfo info = new(cnt + 10, spawn_tPos, true, agentsList[cnt]);
                agentsData.Add(info);

                // For debug
                if (settings.debugMode) info.PrintAgentInfo();

                cnt++;
            }
        }
    }


    /// <summary>
    /// Move agent_tile in the direction specified by dir.
    /// </summary>
    /// <param name="agent_id">Agent's id</param>
    /// <param name="dir">Direction of moving</param>
    public void MoveAgentTile(int agent_id, int dir)
    {
        Vector3Int pos = agentsData[agent_id - 10].m_position.m_tilemapPosition;
        Vector3Int newPos = new();

        // Forward
        if (dir == 1) newPos = new(pos.x, pos.y + 1, pos.z);

        // Right
        else if (dir == 2) newPos = new(pos.x + 1, pos.y, pos.z);

        // Back
        else if (dir == 3) newPos = new(pos.x, pos.y - 1, pos.z);

        // Left
        else if (dir == 4) newPos = new(pos.x - 1, pos.y, pos.z);


        agent_tilemap.SetTile(pos, null);
        agent_tilemap.SetTile(newPos, agent_tile);

        agentsData[agent_id - 10].m_position.m_tilemapPosition = newPos;

        // For debug
        if (settings.debugMode) agentsData[agent_id].PrintAgentInfo();
    }


    /// <summary>
    /// Output Field data. (for debug)
    /// </summary>
    /// <param name="height">Field's height</param>
    /// <param name="width">Field's width</param>
    private void PrintFieldData(int height, int width)
    {
        Debug.Log("===== Field data =====");
        for (int y = 0; y < height; y++)
        {
            string s = string.Empty;
            for (int x = 0; x < width; x++)
            {
                s += fieldData2D[y][x].tileNum.ToString() + " ";
            }
            Debug.Log(s);
        }
        Debug.Log("======================");
    }
}
