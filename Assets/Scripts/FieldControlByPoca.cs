using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Tilemaps;
using Unity.MLAgents;

public class FieldControlByPoca : MonoBehaviour
{
    // Settings
    public GameObject settings_obj;
    Settings settings;


    // FieldDataReader
    public GameObject fieldDataReader_obj;
    FieldDataReader fieldDataReader;

    [Header("Max Environment Steps")] public int maxEnvironmentSteps = 1000;
    private int m_resetTimer;

    // FieldData
    [HideInInspector] public List<List<int>> fieldData;
    [HideInInspector] public List<List<bool>> fieldAgentData;

    // Tilemap resources
    public List<Sprite> tilemapSprites;
    public List<TileBase> tiles;
    public Tilemap field_tilemap;
    public Tilemap agent_tilemap;

    // Agents resources
    [HideInInspector] public int activeAgentsNum;
    public List<GameObject> agentsList;
    [HideInInspector] public List<AgentInfo> agentsInfo;
    private SimpleMultiAgentGroup m_agentGroup;


    private void Start()
    {
        settings = settings_obj.GetComponent<Settings>();
        fieldDataReader = fieldDataReader_obj.GetComponent<FieldDataReader>();

        ResetFieldData(settings.fieldHeight, settings.fieldWidth);
        InitializeTileMaps(settings.fieldHeight, settings.fieldWidth);

        m_agentGroup = new SimpleMultiAgentGroup();
        foreach (var item in agentsInfo)
        {
            m_agentGroup.RegisterAgent(item.m_agentControl);
        }
    }


    private void FixedUpdate()
    {
        m_resetTimer++;
        if (m_resetTimer >= maxEnvironmentSteps && maxEnvironmentSteps > 0)
        {
            m_agentGroup.GroupEpisodeInterrupted();
            InitializeTileMaps(settings.fieldHeight, settings.fieldWidth);
        }

        m_agentGroup.AddGroupReward(-1.0f / maxEnvironmentSteps);
    }


    /// <summary>
    /// Reset Field data.
    /// </summary>
    /// <param name="height">Height of the field</param>
    /// <param name="width">Width of the field</param>
    private void ResetFieldData(int height, int width)
    {
        fieldData = fieldDataReader.m_fieldData;
        fieldAgentData = new();
        for (int y = 0; y < height; y++)
        {
            List<bool> temp = new();
            for (int x = 0; x < width; x++)
            {
                temp.Add(false);
            }
            fieldAgentData.Add(temp);
        }

        agent_tilemap.ClearAllTiles();
        field_tilemap.ClearAllTiles();
        SetFieldTilemap(height, width);

        // For debug
        if (settings.debugMode) PrintFieldData(height, width);
    }


    /// <summary>
    /// Initializing tilemaps.
    /// </summary>
    /// <param name="height">Height of the field</param>
    /// <param name="width">Width of the field</param>
    public void InitializeTileMaps(int height, int width)
    {
        activeAgentsNum = settings.agentCnt;
        m_resetTimer = 0;
        agent_tilemap.ClearAllTiles();
        RandomSetAgent(height, width, settings.agentCnt);
    }


    /// <summary>
    /// Set tiles of field.
    /// </summary>
    /// <param name="height">Height of the field</param>
    /// <param name="width">Width of the field</param>
    private void SetFieldTilemap(int height, int width)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                TileBase tile = null;
                if (fieldData[y][x] == 0) tile = null;
                if (fieldData[y][x] == 1) tile = tiles[0];
                if (fieldData[y][x] == 2) tile = tiles[1];
                if (fieldData[y][x] == 3) tile = tiles[2];
                if (fieldData[y][x] == 4) tile = tiles[4];

                field_tilemap.SetTile(new Vector3Int(x, height - y, 0), tile);
            }
        }
    }


    /// <summary>
    /// Set agents to random place in the field.
    /// </summary>
    /// <param name="height">Height of the field</param>
    /// <param name="width">Width of the field</param>
    /// <param name="agentNum">Number of Agents</param>
    private void RandomSetAgent(int height, int width, int agentNum)
    {
        agentsInfo = new();

        int cnt = 0;
        while (cnt < agentNum)
        {
            Vector2Int spawnIndex = new(Random.Range(0, width), Random.Range(0, height));

            // Only Empty position
            if (fieldData[spawnIndex.y][spawnIndex.x] == 1)
            {
                agent_tilemap.SetTile(new Vector3Int(spawnIndex.x, height - spawnIndex.y, 0), tiles[3]);

                AgentControlByPoca agentControl = agentsList[cnt].GetComponent<AgentControlByPoca>();
                AgentInfo info = new(cnt + 10, spawnIndex, true, agentsList[cnt], agentControl);
                agentsInfo.Add(info);
                fieldAgentData[spawnIndex.y][spawnIndex.x] = true;

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
        int posIndex_x = agentsInfo[agent_id - 10].m_positionIndex.x;
        int posIndex_y = agentsInfo[agent_id - 10].m_positionIndex.y;
        Vector3Int pos = new(posIndex_x, settings.fieldHeight - posIndex_y, 0);

        agent_tilemap.SetTile(pos, null);
        fieldAgentData[posIndex_y][posIndex_x] = false;

        // Forward
        if (dir == 1)
        {
            if (!fieldAgentData[posIndex_y - 1][posIndex_x])
            {
                posIndex_y--;
            }
        }

        // Back
        else if (dir == 2)
        {
            if (!fieldAgentData[posIndex_y + 1][posIndex_x])
            {
                posIndex_y++;
            }
        }

        // Right
        else if (dir == 3)
        {
            if (!fieldAgentData[posIndex_y][posIndex_x + 1])
            {
                posIndex_x++;
            }
        }

        // Left
        else if (dir == 4)
        {
            if (!fieldAgentData[posIndex_y][posIndex_x - 1])
            {
                posIndex_x--;
            }
        }

        agentsInfo[agent_id - 10].m_positionIndex.x = posIndex_x;
        agentsInfo[agent_id - 10].m_positionIndex.y = posIndex_y;
        pos = new(posIndex_x, settings.fieldHeight - posIndex_y, 0);

        agent_tilemap.SetTile(pos, tiles[3]);
        fieldAgentData[posIndex_y][posIndex_x] = true;

        // For debug
        if (settings.debugMode) agentsInfo[agent_id - 10].PrintAgentInfo();
    }


    /// <summary>
    /// When agent reached any exit.
    /// </summary>
    public void ReachedExit()
    {
        m_agentGroup.AddGroupReward(1.0f);
    }


    /// <summary>
    /// When all agents reached any exit.
    /// </summary>
    public void AllReachedExit()
    {
        m_agentGroup.EndGroupEpisode();
    }


    /// <summary>
    /// Output Field data. (for debug)
    /// </summary>
    /// <param name="height">Height of the field</param>
    /// <param name="width">Width of the field</param>
    private void PrintFieldData(int height, int width)
    {
        Debug.Log("===== Field data =====");
        for (int y = 0; y < height; y++)
        {
            string s = string.Empty;
            for (int x = 0; x < width; x++)
            {
                s += fieldData[y][x].ToString() + " ";
            }
            Debug.Log(s);
        }
        Debug.Log("======================");
    }
}
