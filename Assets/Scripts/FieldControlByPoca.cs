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
    private bool dataCounterInitialized = false;

    // CSVExporter
    public GameObject csvExporter_obj;
    CsvExporter csvExporter;
    private int episodeCount;

    // DataCounter
    public GameObject dataCounter_obj;
    DataCounter dataCounter;

    [Header("Max Environment Steps")] public int maxEnvironmentSteps = 1000;
    private int m_resetTimer;

    // FieldData
    [HideInInspector] public List<List<List<int>>> fieldDataList;
    [HideInInspector] public List<List<List<bool>>> fieldAgentDataList;

    // Tilemap resources
    public List<Sprite> tilemapSprites;
    public List<TileBase> tiles;
    public List<Tilemap> fieldTilemapList;
    public List<Tilemap> agentTilemapList;

    // Agents resources
    [HideInInspector] public int activeAgentsNum;
    public List<GameObject> agentsList;
    [HideInInspector] public List<AgentInfo> agentsInfo;
    private SimpleMultiAgentGroup m_agentGroup;


    private void Start()
    {
        settings = settings_obj.GetComponent<Settings>();
        fieldDataReader = fieldDataReader_obj.GetComponent<FieldDataReader>();
        dataCounter = dataCounter_obj.GetComponent<DataCounter>();
        csvExporter = csvExporter_obj.GetComponent<CsvExporter>();

        if (!dataCounterInitialized && settings.dataCountMode)
        {
            Time.timeScale = settings.timeScale;

            dataCounter.Initialize(settings.fieldHeight, settings.fieldWidth);
            dataCounterInitialized = true;
            episodeCount = 0;
        }

        ResetFieldData(settings.fieldHeight, settings.fieldWidth);
        InitializeTileMaps(settings.fieldHeight, settings.fieldWidth);

        m_agentGroup = new SimpleMultiAgentGroup();
        foreach (var item in agentsInfo)
        {
            m_agentGroup.RegisterAgent(item.agentControl);
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
        // For fieldAgentData
        fieldAgentDataList = new();
        for (int i = 0; i < agentTilemapList.Count; i++)
        {
            List<List<bool>> temp = new();
            for (int y = 0; y < height; y++)
            {
                List<bool> temp2 = new();
                for (int x = 0; x < width; x++)
                {
                    temp2.Add(false);
                }
                temp.Add(temp2);
            }
            fieldAgentDataList.Add(temp);
        }

        foreach (var tilemap in agentTilemapList)
        {
            tilemap.ClearAllTiles();
        }

        // For fieldDataList
        fieldDataList = fieldDataReader.fieldDataList;
        for (int i = 0; i < fieldDataList.Count; i++)
        {
            fieldTilemapList[i].ClearAllTiles();
            SetFieldTilemap(i, height, width);

            // For debug
            if (settings.debugMode) PrintFieldData(i, height, width);
        }
    }


    /// <summary>
    /// Initializing tilemaps.
    /// </summary>
    /// <param name="height">Height of the field</param>
    /// <param name="width">Width of the field</param>
    public void InitializeTileMaps(int height, int width)
    {
        if (settings.dataCountMode && episodeCount > settings.maxEpisodeCount)
        {
            csvExporter.SaveData(dataCounter.fieldData);
            settings.dataCountMode = false;
        }

        activeAgentsNum = settings.agentCnt;
        m_resetTimer = 0;

        foreach (var tilemap in agentTilemapList)
        {
            tilemap.ClearAllTiles();
        }

        RandomSetAgent(height, width, settings.agentCnt);

        if (settings.dataCountMode) episodeCount++;
    }


    /// <summary>
    /// Set tiles of field.
    /// </summary>
    /// <param name="index">Index of data</param>
    /// <param name="height">Height of the field</param>
    /// <param name="width">Width of the field</param>
    private void SetFieldTilemap(int index, int height, int width)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                TileBase tile = null;
                // Null
                if (fieldDataList[index][y][x] == 0)
                {
                    tile = tiles[0];
                }

                // Empty
                else if (fieldDataList[index][y][x] == 1) 
                {
                    tile = tiles[0];
                }

                // Exit
                else if (fieldDataList[index][y][x] == 2) 
                { 
                    tile = tiles[1];
                }

                // Obstacle
                else if (fieldDataList[index][y][x] == 3)
                {
                    tile = tiles[2];
                }

                // Upstair
                else if (fieldDataList[index][y][x] == 4) 
                {
                    //tile = tiles[4];
                    tile = tiles[2];
                }

                // Downstair
                else if (fieldDataList[index][y][x] == 5) 
                {
                    tile = tiles[5];
                }

                fieldTilemapList[index].SetTile(new Vector3Int(x, height - y, 0), tile);
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
            //int spawnFloor = Random.Range(0, 5 + 1);
            int spawnFloor = 0;
            Vector2Int spawnPosIndex = new(Random.Range(0, width), Random.Range(0, height));

            // Only Empty position
            if (fieldDataList[0][spawnPosIndex.y][spawnPosIndex.x] == 1)
            {
                agentTilemapList[spawnFloor].SetTile(new Vector3Int(spawnPosIndex.x, height - spawnPosIndex.y, 0), tiles[3]);

                AgentControlByPoca agentControl = agentsList[cnt].GetComponent<AgentControlByPoca>();
                AgentInfo info = new(cnt + 1000, spawnFloor, spawnPosIndex, true, agentsList[cnt], agentControl);
                agentsInfo.Add(info);
                fieldAgentDataList[spawnFloor][spawnPosIndex.y][spawnPosIndex.x] = true;

                // For debug
                if (settings.debugMode) info.PrintAgentInfo();

                cnt++;
            }
        }
    }


    /// <summary>
    /// Move agent_tile in the direction specified by dir.
    /// </summary>
    /// <param name="floorNum">Agent's current floor</param>
    /// <param name="agent_id">Agent's id</param>
    /// <param name="dir">Direction of moving</param>
    public void MoveAgentTile(int floorNum, int agent_id, int dir)
    {
        int posIndex_x = agentsInfo[agent_id - 1000].positionIndex.x;
        int posIndex_y = agentsInfo[agent_id - 1000].positionIndex.y;
        Vector3Int pos = new(posIndex_x, settings.fieldHeight - posIndex_y, 0);

        agentTilemapList[floorNum].SetTile(pos, null);
        fieldAgentDataList[floorNum][posIndex_y][posIndex_x] = false;

        // Forward
        if (dir == 1)
        {
            if (!fieldAgentDataList[floorNum][posIndex_y - 1][posIndex_x])
            {
                posIndex_y--;
            }
        }

        // Back
        else if (dir == 2)
        {
            if (!fieldAgentDataList[floorNum][posIndex_y + 1][posIndex_x])
            {
                posIndex_y++;
            }
        }

        // Right
        else if (dir == 3)
        {
            if (!fieldAgentDataList[floorNum][posIndex_y][posIndex_x + 1])
            {
                posIndex_x++;
            }
        }

        // Left
        else if (dir == 4)
        {
            if (!fieldAgentDataList[floorNum][posIndex_y][posIndex_x - 1])
            {
                posIndex_x--;
            }
        }

        agentsInfo[agent_id - 1000].positionIndex.x = posIndex_x;
        agentsInfo[agent_id - 1000].positionIndex.y = posIndex_y;
        pos = new(posIndex_x, settings.fieldHeight - posIndex_y, 0);

        if (settings.dataCountMode) dataCounter.UpdateData(posIndex_y, posIndex_x);

        agentTilemapList[floorNum].SetTile(pos, tiles[3]);
        fieldAgentDataList[floorNum][posIndex_y][posIndex_x] = true;

        // For debug
        if (settings.debugMode) agentsInfo[agent_id - 1000].PrintAgentInfo();
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
    /// <param name="index">Index of data</param>
    /// <param name="height">Height of the field</param>
    /// <param name="width">Width of the field</param>
    private void PrintFieldData(int index, int height, int width)
    {
        Debug.Log("===== Field data =====");
        for (int idx = 0; idx < fieldDataList.Count; idx++)
        {
            Debug.Log($"[Floor] : {idx + 1}F");
            for (int y = 0; y < height; y++)
            {
                string s = string.Empty;
                for (int x = 0; x < width; x++)
                {
                    s += fieldDataList[index][y][x].ToString() + " ";
                }
                Debug.Log(s);
            }
        }
        Debug.Log("======================");
    }
}
