using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Tilemaps;

public class FieldControl : MonoBehaviour
{
    // Settings
    public GameObject settings_obj;
    Settings settings;

    // FieldDataReader
    public GameObject fieldDataReader_obj;
    FieldDataReader fieldDataReader;

    // FieldData
    [HideInInspector] public List<List<int>> fieldData2D;

    // Tilemaps resources
    public List<Sprite> tilemapSprites;
    public List<TileBase> tiles;
    public Tilemap field_tilemap;
    public Tilemap agent_tilemap;

    // Agents resources
    [HideInInspector] public int activeAgentsNum;
    public List<GameObject> agentsList;
    public List<AgentInfo> agentsInfo;


    private void Start()
    {
        settings = settings_obj.GetComponent<Settings>();
        fieldDataReader = fieldDataReader_obj.GetComponent<FieldDataReader>();
        activeAgentsNum = settings.agentCnt;

        ResetFieldData();
        InitializeTileMaps();
    }


    /// <summary>
    /// Reset Field data.
    /// </summary>
    private void ResetFieldData()
    {
        fieldData2D = fieldDataReader.m_fieldData;

        // For debug
        if (settings.debugMode) PrintFieldData(settings.fieldHeight, settings.fieldWidth);
    }


    /// <summary>
    /// Initializing tilemaps.
    /// </summary>
    public void InitializeTileMaps()
    {
        field_tilemap.ClearAllTiles();
        agent_tilemap.ClearAllTiles();
        SetFieldTilemap(settings.fieldHeight, settings.fieldWidth);
        RandomSetAgent(settings.fieldHeight, settings.fieldWidth, settings.agentCnt);
    }


    private void SetFieldTilemap(int height, int width)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                TileBase tile = null;
                if (fieldData2D[y][x] == 0) tile = null;
                if (fieldData2D[y][x] == 1) tile = tiles[0];
                if (fieldData2D[y][x] == 2) tile = tiles[1];
                if (fieldData2D[y][x] == 3) tile = tiles[2];

                field_tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
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
        agentsInfo = new();

        int cnt = 0;
        while (cnt < agentNum)
        {
            Vector2Int spawnIndex = new(Random.Range(0, width), Random.Range(0, height));

            // Only Empty position
            if (fieldData2D[spawnIndex.y][spawnIndex.x] == 1)
            {
                agent_tilemap.SetTile(new Vector3Int(spawnIndex.x, spawnIndex.y, 0), tiles[3]);

                AgentInfo info = new(cnt + 10, spawnIndex, true, agentsList[cnt]);
                agentsInfo.Add(info);

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
        Vector3Int pos = new(agentsInfo[agent_id - 10].m_positionIndex.x, agentsInfo[agent_id - 10].m_positionIndex.y, 0);
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
        agent_tilemap.SetTile(newPos, tiles[3]);

        agentsInfo[agent_id - 10].m_positionIndex = (Vector2Int)newPos;

        // For debug
        if (settings.debugMode) agentsInfo[agent_id].PrintAgentInfo();
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
                s += fieldData2D[y][x].ToString() + " ";
            }
            Debug.Log(s);
        }
        Debug.Log("======================");
    }
}
