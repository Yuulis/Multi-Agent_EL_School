using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Tilemaps;

public class FieldControl : MonoBehaviour
{
    public GameObject Settings_obj;
    Settings settings;

    public GameObject TrainingArea;

    private bool firstEpisode;

    [HideInInspector] public List<List<int>> fieldData;

    public List<Sprite> tilemapSprites;
    public Tilemap field_tilemap;
    public Tilemap agent_tilemap;
    public TileBase agent_tile;

    [HideInInspector] public int activeAgentsNum;
    public List<GameObject> agentsList;
    public List<AgentInfo> agentsData;


    private void Awake()
    {
        settings = Settings_obj.GetComponent<Settings>();
        activeAgentsNum = settings.agentCnt;

        firstEpisode = true;

        ResetFieldData(settings.fieldWidth, settings.fieldHeight);

        // For debug
        if (settings.debugMode) PrintFieldData(settings.fieldWidth, settings.fieldHeight);

        InitializeField();

        firstEpisode = false;
    }


    /// <summary>
    /// Initializing Field data.
    /// </summary>
    public void InitializeField()
    {
        agent_tilemap.ClearAllTiles();
        RandomSetAgent(settings.fieldWidth, settings.fieldHeight, settings.agentCnt);
    }


    /// <summary>
    /// Reset Field data.
    /// </summary>
    /// <param name="width">Field's width</param>
    /// <param name="height">Field's height</param>
    private void ResetFieldData(int width, int height)
    {
        fieldData = new();

        for (int i = 0; i < height; i++)
        {
            List<int> temp = new();
            for (int j = 0; j < width; j++)
            {
                temp.Add(0);
            }
            fieldData.Add(temp);
        }

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                TilemapPosition tPos = new();
                tPos.m_tilemapData_x = j;
                tPos.m_tilemapData_y = i;
                tPos.m_tilemap_pos = tPos.ChangeFICStoTCCS(tPos.m_tilemapData_x, tPos.m_tilemapData_y, height, width);

                // Empty
                if (field_tilemap.GetSprite(tPos.m_tilemap_pos) == tilemapSprites[1])
                {
                    fieldData[i][j] = 1;
                }

                // Exit
                else if (field_tilemap.GetSprite(tPos.m_tilemap_pos) == tilemapSprites[2])
                {
                    fieldData[i][j] = 2;
                }

                // Obstacle
                else if (field_tilemap.GetSprite(tPos.m_tilemap_pos) == tilemapSprites[3])
                {
                    fieldData[i][j] = 3;
                }
            }
        }
    }


    /// <summary>
    /// Agents set random place in field.
    /// </summary>
    /// <param name="width">Field's width</param>
    /// <param name="height">Field's height</param>
    /// <param name="num">Number of Agents</param>
    private void RandomSetAgent(int width, int height, int num)
    {
        agentsData = new();

        int cnt = 0;
        while (cnt < num)
        {
            Vector3Int spawPos = new(Random.Range(-width / 2, width / 2), Random.Range(height / 2 - 1, -height / 2 + 1), 0);
            
            // Only Empty position
            if (field_tilemap.GetSprite(spawPos) == tilemapSprites[1] && agent_tilemap.GetTile(spawPos) != agent_tile)
            {
                agent_tilemap.SetTile(spawPos, agent_tile);

                TilemapPosition tPos = new();
                tPos.m_tilemap_pos = spawPos;
                (tPos.m_tilemapData_x, tPos.m_tilemapData_y) = tPos.ChangeTCCStoFICS(tPos.m_tilemap_pos, height, width);

                fieldData[tPos.m_tilemapData_y][tPos.m_tilemapData_x] = 10 + cnt;

                AgentInfo info = new(cnt + 10, tPos, true, agentsList[cnt]);
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
        Vector3Int pos = agentsData[agent_id - 10].m_position.m_tilemap_pos;
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

        agentsData[agent_id - 10].m_position.m_tilemap_pos = newPos;

        // For debug
        if (settings.debugMode) agentsData[agent_id].PrintAgentInfo();
    }


    /// <summary>
    /// Output Field data(for debug).
    /// </summary>
    /// <param name="width">Field's width</param>
    /// <param name="height">Field's height</param>
    private void PrintFieldData(int width, int height)
    {
        Debug.Log("===== Field data =====");
        for (int i = 0; i < height; i++)
        {
            string s = string.Empty;
            for (int j = 0; j < width; j++)
            {
                s += fieldData[i][j].ToString() + " ";
            }
            Debug.Log(s);
        }
        Debug.Log("======================");
    }
}
