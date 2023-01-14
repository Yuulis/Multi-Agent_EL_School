using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class FieldControlMultiFloor : MonoBehaviour
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
    private int resetTimer;

    // FieldData
    [HideInInspector] public List<List<List<CellInfo>>> fieldDataList;
    [HideInInspector] public List<List<List<bool>>> fieldAgentDataList;
    [HideInInspector] public List<Tuple<int, Vector2Int, int, Vector2Int>> stairDataList;

    // Tilemap resources
    public List<Sprite> tilemapSprites;
    public List<TileBase> tiles;
    public List<Tilemap> fieldTilemapList;
    public List<Tilemap> agentTilemapList;

    // Agents resources
    [HideInInspector] public int activeAgentsNum;
    public List<GameObject> agentsList;
    [HideInInspector] public List<AgentInfoMultiFloor> agentsInfo;
    private SimpleMultiAgentGroup agentGroup;


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

        InitializeStairDataList();
        ResetFieldData(settings.fieldHeight, settings.fieldWidth);
        InitializeTileMaps(settings.fieldHeight, settings.fieldWidth);

        agentGroup = new SimpleMultiAgentGroup();
        foreach (var item in agentsInfo)
        {
            agentGroup.RegisterAgent(item.agentControl);
        }
    }


    private void FixedUpdate()
    {
        resetTimer++;
        if (resetTimer >= maxEnvironmentSteps && maxEnvironmentSteps > 0)
        {
            agentGroup.GroupEpisodeInterrupted();
            InitializeTileMaps(settings.fieldHeight, settings.fieldWidth);
        }

        agentGroup.AddGroupReward(-1.0f / maxEnvironmentSteps);
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
        fieldDataList = new();
        for (int index = 0; index < fieldTilemapList.Count; index++)
        {
            List<List<CellInfo>> temp1 = new();
            for (int y = 0; y < settings.fieldHeight; y++)
            {
                List<CellInfo> temp2 = new();
                for (int x = 0; x < settings.fieldWidth; x++)
                {
                    int cellType = fieldDataReader.fieldDataList[index][y][x];
                    StairInfo stairInfo = new(0, new Vector2Int(0, 0), 0, new Vector2Int(0, 0));

                    // If cell is upstair or downstair
                    if (cellType == 4 || cellType == 5)
                    {
                        foreach (var stairData in stairDataList)
                        {
                            if (stairData.Item1 == index && stairData.Item2.y == y && stairData.Item2.x == x)
                            {
                                stairInfo = new(stairData.Item1, stairData.Item2, stairData.Item3, stairData.Item4);
                                break;
                            }
                        }
                    }

                    CellInfo cellInfo = new(cellType, stairInfo);
                    temp2.Add(cellInfo);
                }

                temp1.Add(temp2);
            }

            fieldDataList.Add(temp1);

        }

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
        resetTimer = 0;

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
                if (fieldDataList[index][y][x].cellType == 0) tile = tiles[0];
                if (fieldDataList[index][y][x].cellType == 1) tile = tiles[0];
                if (fieldDataList[index][y][x].cellType == 2) tile = tiles[1];
                if (fieldDataList[index][y][x].cellType == 3) tile = tiles[2];
                if (fieldDataList[index][y][x].cellType == 4) tile = tiles[4];
                if (fieldDataList[index][y][x].cellType == 5) tile = tiles[5];

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
            if (fieldDataList[0][spawnPosIndex.y][spawnPosIndex.x].cellType == 1)
            {
                agentTilemapList[spawnFloor].SetTile(new Vector3Int(spawnPosIndex.x, height - spawnPosIndex.y, 0), tiles[3]);

                AgentControlMultiFloor agentControl = agentsList[cnt].GetComponent<AgentControlMultiFloor>();
                AgentInfoMultiFloor info = new(cnt + 1000, spawnFloor, spawnPosIndex, true, false, agentsList[cnt], agentControl);
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
        agentTilemapList[floorNum].SetTile(new Vector3Int(posIndex_x, settings.fieldHeight - posIndex_y, 0), null);
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


        // If agent reaches stair, it moves another floor
        if ((fieldDataList[floorNum][posIndex_y][posIndex_x].cellType == 4 || fieldDataList[floorNum][posIndex_y][posIndex_x].cellType == 5) && !agentsInfo[agent_id - 1000].usedStair)
        {
            StairInfo stairInfo = fieldDataList[floorNum][posIndex_y][posIndex_x].stairInfo;
            floorNum = stairInfo.floorTo;
            posIndex_x = stairInfo.posTo.x;
            posIndex_y = stairInfo.posTo.y;

            agentsInfo[agent_id - 1000].usedStair = true;
        }
        else
        {
            agentsInfo[agent_id - 1000].usedStair = false;
        }

        agentsInfo[agent_id - 1000].positionIndex.x = posIndex_x;
        agentsInfo[agent_id - 1000].positionIndex.y = posIndex_y;
        agentsInfo[agent_id - 1000].floorNum = floorNum;
        agentTilemapList[floorNum].SetTile(new Vector3Int(posIndex_x, settings.fieldHeight - posIndex_y, 0), tiles[3]);
        fieldAgentDataList[floorNum][posIndex_y][posIndex_x] = true;

        if (settings.dataCountMode) dataCounter.UpdateData(posIndex_y, posIndex_x);

        // For debug
        if (settings.debugMode) agentsInfo[agent_id - 1000].PrintAgentInfo();
    }


    /// <summary>
    /// When agent reached any exit.
    /// </summary>
    public void ReachedExit()
    {
        agentGroup.AddGroupReward(1.0f);
    }


    /// <summary>
    /// When all agents reached any exit.
    /// </summary>
    public void AllReachedExit()
    {
        agentGroup.EndGroupEpisode();
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


    /// <summary>
    /// Initialize stairDataList.
    /// </summary>
    private void InitializeStairDataList()
    {
        stairDataList = new()
        {
            // 1F-1
            new Tuple<int, Vector2Int, int, Vector2Int>(0, new Vector2Int(12, 21), 1, new Vector2Int(15, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(0, new Vector2Int(13, 21), 1, new Vector2Int(14, 21)),

            // 1F-2
            new Tuple<int, Vector2Int, int, Vector2Int>(0, new Vector2Int(51, 21), 1, new Vector2Int(51, 14)),
            new Tuple<int, Vector2Int, int, Vector2Int>(0, new Vector2Int(52, 21), 1, new Vector2Int(52, 14)),

            // 1F-3
            new Tuple<int, Vector2Int, int, Vector2Int>(0, new Vector2Int(87, 21), 1, new Vector2Int(86, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(0, new Vector2Int(88, 21), 1, new Vector2Int(85, 21)),

            // 2F-1
            new Tuple<int, Vector2Int, int, Vector2Int>(1, new Vector2Int(12, 21), 2, new Vector2Int(15, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(1, new Vector2Int(13, 21), 2, new Vector2Int(14, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(1, new Vector2Int(15, 21), 0, new Vector2Int(12, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(1, new Vector2Int(14, 21), 0, new Vector2Int(13, 21)),

            // 2F-2
            new Tuple<int, Vector2Int, int, Vector2Int>(1, new Vector2Int(51, 21), 2, new Vector2Int(51, 14)),
            new Tuple<int, Vector2Int, int, Vector2Int>(1, new Vector2Int(52, 21), 2, new Vector2Int(52, 14)),
            new Tuple<int, Vector2Int, int, Vector2Int>(1, new Vector2Int(51, 14), 0, new Vector2Int(51, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(1, new Vector2Int(52, 14), 0, new Vector2Int(52, 21)),

            // 2F-3
            new Tuple<int, Vector2Int, int, Vector2Int>(1, new Vector2Int(87, 21), 2, new Vector2Int(86, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(1, new Vector2Int(88, 21), 2, new Vector2Int(75, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(1, new Vector2Int(86, 21), 0, new Vector2Int(87, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(1, new Vector2Int(85, 21), 0, new Vector2Int(88, 21)),

            // Stair-field (2F)
            new Tuple<int, Vector2Int, int, Vector2Int>(1, new Vector2Int(70, 13), 2, new Vector2Int(70, 13)),
            new Tuple<int, Vector2Int, int, Vector2Int>(1, new Vector2Int(70, 14), 2, new Vector2Int(70, 14)),
            new Tuple<int, Vector2Int, int, Vector2Int>(1, new Vector2Int(70, 15), 2, new Vector2Int(70, 15)),
            new Tuple<int, Vector2Int, int, Vector2Int>(1, new Vector2Int(70, 16), 2, new Vector2Int(70, 16)),
            new Tuple<int, Vector2Int, int, Vector2Int>(1, new Vector2Int(70, 17), 2, new Vector2Int(70, 17)),
            new Tuple<int, Vector2Int, int, Vector2Int>(1, new Vector2Int(70, 18), 2, new Vector2Int(70, 18)),
            new Tuple<int, Vector2Int, int, Vector2Int>(1, new Vector2Int(70, 19), 2, new Vector2Int(70, 19)),
            new Tuple<int, Vector2Int, int, Vector2Int>(1, new Vector2Int(70, 20), 2, new Vector2Int(70, 20)),
            new Tuple<int, Vector2Int, int, Vector2Int>(1, new Vector2Int(70, 21), 2, new Vector2Int(70, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(1, new Vector2Int(70, 22), 2, new Vector2Int(70, 22)),

            // 3F-1
            new Tuple<int, Vector2Int, int, Vector2Int>(2, new Vector2Int(12, 21), 3, new Vector2Int(15, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(2, new Vector2Int(13, 21), 3, new Vector2Int(14, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(2, new Vector2Int(15, 21), 1, new Vector2Int(12, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(2, new Vector2Int(14, 21), 1, new Vector2Int(13, 21)),

            // 3F-2
            new Tuple<int, Vector2Int, int, Vector2Int>(2, new Vector2Int(51, 21), 3, new Vector2Int(51, 14)),
            new Tuple<int, Vector2Int, int, Vector2Int>(2, new Vector2Int(52, 21), 3, new Vector2Int(52, 14)),
            new Tuple<int, Vector2Int, int, Vector2Int>(2, new Vector2Int(51, 14), 1, new Vector2Int(51, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(2, new Vector2Int(52, 14), 1, new Vector2Int(52, 21)),

            // 3F-3
            new Tuple<int, Vector2Int, int, Vector2Int>(2, new Vector2Int(87, 21), 3, new Vector2Int(86, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(2, new Vector2Int(88, 21), 3, new Vector2Int(75, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(2, new Vector2Int(86, 21), 1, new Vector2Int(87, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(2, new Vector2Int(85, 21), 1, new Vector2Int(88, 21)),

            // Stair-field (3F)
            new Tuple<int, Vector2Int, int, Vector2Int>(2, new Vector2Int(70, 13), 1, new Vector2Int(70, 13)),
            new Tuple<int, Vector2Int, int, Vector2Int>(2, new Vector2Int(70, 14), 1, new Vector2Int(70, 14)),
            new Tuple<int, Vector2Int, int, Vector2Int>(2, new Vector2Int(70, 15), 1, new Vector2Int(70, 15)),
            new Tuple<int, Vector2Int, int, Vector2Int>(2, new Vector2Int(70, 16), 1, new Vector2Int(70, 16)),
            new Tuple<int, Vector2Int, int, Vector2Int>(2, new Vector2Int(70, 17), 1, new Vector2Int(70, 17)),
            new Tuple<int, Vector2Int, int, Vector2Int>(2, new Vector2Int(70, 18), 1, new Vector2Int(70, 18)),
            new Tuple<int, Vector2Int, int, Vector2Int>(2, new Vector2Int(70, 19), 1, new Vector2Int(70, 19)),
            new Tuple<int, Vector2Int, int, Vector2Int>(2, new Vector2Int(70, 20), 1, new Vector2Int(70, 20)),
            new Tuple<int, Vector2Int, int, Vector2Int>(2, new Vector2Int(70, 21), 1, new Vector2Int(70, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(2, new Vector2Int(70, 22), 1, new Vector2Int(70, 22)),

            // 4F-1
            new Tuple<int, Vector2Int, int, Vector2Int>(3, new Vector2Int(12, 21), 4, new Vector2Int(15, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(3, new Vector2Int(13, 21), 4, new Vector2Int(14, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(3, new Vector2Int(15, 21), 2, new Vector2Int(12, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(3, new Vector2Int(14, 21), 2, new Vector2Int(13, 21)),

            // 4F-2
            new Tuple<int, Vector2Int, int, Vector2Int>(3, new Vector2Int(51, 21), 4, new Vector2Int(51, 14)),
            new Tuple<int, Vector2Int, int, Vector2Int>(3, new Vector2Int(52, 21), 4, new Vector2Int(52, 14)),
            new Tuple<int, Vector2Int, int, Vector2Int>(3, new Vector2Int(51, 14), 2, new Vector2Int(51, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(3, new Vector2Int(52, 14), 2, new Vector2Int(52, 21)),

            // 4F-3
            new Tuple<int, Vector2Int, int, Vector2Int>(3, new Vector2Int(87, 21), 4, new Vector2Int(86, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(3, new Vector2Int(88, 21), 4, new Vector2Int(75, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(3, new Vector2Int(86, 21), 2, new Vector2Int(87, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(3, new Vector2Int(85, 21), 2, new Vector2Int(88, 21)),

            // 5F-1
            new Tuple<int, Vector2Int, int, Vector2Int>(4, new Vector2Int(15, 21), 3, new Vector2Int(12, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(4, new Vector2Int(14, 21), 3, new Vector2Int(13, 21)),

            // 5F-2
            new Tuple<int, Vector2Int, int, Vector2Int>(4, new Vector2Int(51, 14), 3, new Vector2Int(51, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(4, new Vector2Int(52, 14), 3, new Vector2Int(52, 21)),

            // 5F-3
            new Tuple<int, Vector2Int, int, Vector2Int>(4, new Vector2Int(86, 21), 3, new Vector2Int(87, 21)),
            new Tuple<int, Vector2Int, int, Vector2Int>(4, new Vector2Int(85, 21), 3, new Vector2Int(88, 21)),
        };
    }
}
