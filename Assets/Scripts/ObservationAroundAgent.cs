using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObservationAroundAgent
{
    // Agent's observation
    public List<List<int>> observationList;

    // Agent's observation of around itself (only detecting whether the tile is null or not; the empty tile and exit tile is true)
    public List<bool> observationListNeighborhood;

    private readonly List<List<int>> fieldDataList;
    private readonly List<List<bool>> fieldAgentData;
    private readonly int fieldHeight;
    private readonly int fieldWidth;
    private Vector2Int positionIndex;
    private readonly int agentSight;


    /// <summary>
    /// Create observation list.
    /// </summary>
    /// <param name="fieldDataList">fieldData</param>
    /// <param name="fieldAgentData">fieldData of agents</param>
    /// <param name="fieldHeight">Height of the field</param>
    /// <param name="fieldWidth">Width of the field</param>
    /// <param name="positionIndex">Index of agent's position index of the fieldData</param>
    /// <param name="agentSight">Agent's sight</param>
    public ObservationAroundAgent(List<List<int>> fieldDataList, List<List<bool>> fieldAgentData, int fieldHeight, int fieldWidth, Vector2Int positionIndex, int agentSight)
    {
        observationList = new();
        observationListNeighborhood = new();
        this.fieldDataList = fieldDataList;
        this.fieldAgentData = fieldAgentData;
        this.fieldHeight = fieldHeight;
        this.fieldWidth = fieldWidth;
        this.positionIndex = positionIndex;
        this.agentSight = agentSight;

        UpdateObservation(this.positionIndex, agentSight);
    }


    /// <summary>
    /// Update observation list.
    /// </summary>
    /// <param name="new_positionIndex">Index of the agent's new position of the fieldData2D<</param>
    /// <param name="sight">Agent's sight</param>
    public void UpdateObservation(Vector2Int new_positionIndex, int sight)
    {
        // Initialize observation list
        observationListNeighborhood.Clear();
        for (int i = 0; i < 9; i++)
        {
            observationListNeighborhood.Add(false);
        }

        observationList.Clear();
        for (int y = 0; y < sight * 2 + 1; y++)
        {
            List<int> temp = new();
            for (int x = 0; x < sight * 2 + 1; x++)
            {
                temp.Add(0);
            }
            observationList.Add(temp);
        }

        positionIndex = new_positionIndex;
        GetObservation(sight);
    }


    /// <summary>
    /// Call GetObservation functions.
    /// <param name="sight">Agent's sight</param>
    /// </summary>
    private void GetObservation(int sight)
    {
        GetObservationNeighborhood(positionIndex, fieldHeight, fieldWidth);
        GetObservationSquareArea(positionIndex, sight, fieldHeight, fieldWidth);
    }

    /// <summary>
    /// Get observation of Agent'S neighborhood(nine tiles)
    /// <param name="positionIndex">Index of agent's position index of the fieldData</param>
    /// <param name="height">Height of the field</param>
    /// <param name="width">Width of the field</param>
    /// </summary>
    public void GetObservationNeighborhood(Vector2Int positionIndex, int height, int width)
    {
        int cnt = 0;
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                // Out of range
                if (positionIndex.y + y < 0 || positionIndex.y + y >= height || positionIndex.x + x < 0 || positionIndex.x + x >= width)
                {
                    observationListNeighborhood[cnt] = false;
                }

                // Agent itself
                else if (y == 0 && x == 0)
                {
                    observationListNeighborhood[cnt] = false;
                }

                // Empty
                else if (fieldDataList[positionIndex.y + y][positionIndex.x + x] == 1)
                {
                    observationListNeighborhood[cnt] = true;
                }

                // Exit
                else if (fieldDataList[positionIndex.y + y][positionIndex.x + x] == 2)
                {
                    observationListNeighborhood[cnt] = true;
                }

                // Obstacle
                else if (fieldDataList[positionIndex.y + y][positionIndex.x + x] == 3)
                {
                    observationListNeighborhood[cnt] = false;
                }

                // Upstair
                else if (fieldDataList[positionIndex.y + y][positionIndex.x + x] == 4)
                {
                    observationListNeighborhood[cnt] = true;
                }

                // Downstair
                else if (fieldDataList[positionIndex.y + y][positionIndex.x + x] == 5)
                {
                    observationListNeighborhood[cnt] = true;
                }

                // Otherwise
                else
                {
                    observationListNeighborhood[cnt] = false;
                }

                cnt++;
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="positionIndex">Index of agent's position index of the fieldData</param>
    /// <param name="sight">Agent's sight</param>
    /// <param name="height">Height of the field</param>
    /// <param name="width">Width of the field</param>
    private void GetObservationSquareArea(Vector2Int positionIndex, int sight, int height, int width)
    {
        for (int y = -sight; y <= sight; y++)
        {
            for (int x = -sight; x <= sight; x++)
            {
                // Out of range
                if (positionIndex.y + y < 0 || positionIndex.y + y >= height || positionIndex.x + x < 0 || positionIndex.x + x >= width)
                {
                    observationList[y + sight][x + sight] = 0;
                }

                // Agent itself
                else if (y == 0 && x == 0)
                {
                    observationList[y + sight][x + sight] = 9;
                }

                // Agent
                else if (fieldAgentData[positionIndex.y + y][positionIndex.x + x])
                {
                    observationList[y + sight][x + sight] = 10;
                }

                // Empty
                else if (fieldDataList[positionIndex.y + y][positionIndex.x + x] == 1)
                {
                    observationList[y + sight][x + sight] = 1;
                }

                // Exit
                else if (fieldDataList[positionIndex.y + y][positionIndex.x + x] == 2)
                {
                    observationList[y + sight][x + sight] = 2;
                }

                // Obstacle
                else if (fieldDataList[positionIndex.y + y][positionIndex.x + x] == 3)
                {
                    observationList[y + sight][x + sight] = 3;
                }

                // Upstair
                else if (fieldDataList[positionIndex.y + y][positionIndex.x + x] == 4)
                {
                    observationList[y + sight][x + sight] = 4;
                }

                // Downstair
                else if (fieldDataList[positionIndex.y + y][positionIndex.x + x] == 5)
                {
                    observationList[y + sight][x + sight] = 5;
                }
            }
        }
    }


    /// <summary>
    /// For debug
    /// </summary>
    /// <param name="agentId">Agent's id</param>
    public void PrintAgentObservation(int agentId)
    {
        Debug.Log($"===== Agent{agentId}'s Observation =====");
        Debug.Log($"[Neighborhood]");
        string s = string.Empty;
        for (int i = 0; i < 9; i++)
        {
            s += observationListNeighborhood[i].ToString() + " ";
        }
        Debug.Log(s);

        Debug.Log($"[Around]");
        for (int y = 0; y < agentSight * 2 + 1; y++)
        {
            s = string.Empty;
            for (int x = 0; x < agentSight * 2 + 1; x++)
            {
                s += observationList[y][x].ToString() + " ";
            }
            Debug.Log(s);
        }
        Debug.Log("=================================");
    }
}
