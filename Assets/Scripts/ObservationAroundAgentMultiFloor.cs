using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObservationAroundAgentMultiFloor
{
    // Agent's observation
    public List<List<int>> observationList;

    // Agent's observation of around itself (only detecting whether the tile is null or not; empty and exit is true)
    public List<bool> observationListNeighborhood;

    private readonly List<List<CellInfo>> fieldDataList;
    private readonly List<List<bool>> fieldAgentData;
    private readonly int fieldHeight;
    private readonly int fieldWidth;
    private Vector2Int positionIndex;
    private readonly int agentSight;
    private bool usedStair;


    /// <summary>
    /// Create observation list.
    /// </summary>
    /// <param name="fieldDataList">fieldData</param>
    /// <param name="fieldAgentData">fieldData of agents (record agent's position) </param>
    /// <param name="fieldHeight">Height of the field</param>
    /// <param name="fieldWidth">Width of the field</param>
    /// <param name="positionIndex">Index of agent's position index of the fieldData</param>
    /// <param name="agentSight">Agent's sight</param>
    /// <param name="usedStair">Whether agent used a stair at previous action or not</param>
    public ObservationAroundAgentMultiFloor(List<List<CellInfo>> fieldDataList, List<List<bool>> fieldAgentData, int fieldHeight, int fieldWidth, Vector2Int positionIndex, int agentSight, bool usedStair)
    {
        observationList = new();
        observationListNeighborhood = new();
        this.fieldDataList = fieldDataList;
        this.fieldAgentData = fieldAgentData;
        this.fieldHeight = fieldHeight;
        this.fieldWidth = fieldWidth;
        this.positionIndex = positionIndex;
        this.agentSight = agentSight;
        this.usedStair = usedStair;

        UpdateObservation(this.positionIndex, agentSight, usedStair);
    }


    /// <summary>
    /// Update observation list.
    /// <param name="new_positionIndex">Index of agent's new position of the fieldData<</param>
    /// <param name="sight">Agent's sight</param>
    /// <param name="usedStair">Whether agent used a stair at previous action or not</param>
    /// </summary>
    public void UpdateObservation(Vector2Int new_positionIndex, int sight, bool usedStair)
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

        // Update data
        positionIndex = new_positionIndex;
        this.usedStair = usedStair;

        GetObservation();
    }


    /// <summary>
    /// Call GetObservation functions.
    /// </summary>
    private void GetObservation()
    {
        GetObservationNeighborhood();
        GetObservationSquareArea();
    }

    /// <summary>
    /// Get observation of Agent's neighborhood (nine tiles)
    /// </summary>
    public void GetObservationNeighborhood()
    {
        int index = 0;
        for (int dy = -1; dy < 2; dy++)
        {
            for (int dx = -1; dx < 2; dx++)
            {
                // Out of range
                if (positionIndex.y + dy < 0 || positionIndex.y + dy >= fieldHeight || positionIndex.x + dx < 0 || positionIndex.x + dx >= fieldWidth)
                {
                    observationListNeighborhood[index] = false;
                }

                // Agent itself
                else if (dy == 0 && dx == 0)
                {
                    observationListNeighborhood[index] = false;
                }

                // Empty
                else if (fieldDataList[positionIndex.y + dy][positionIndex.x + dx].cellType == 1)
                {
                    observationListNeighborhood[index] = true;
                }

                // Exit
                else if (fieldDataList[positionIndex.y + dy][positionIndex.x + dx].cellType == 2)
                {
                    observationListNeighborhood[index] = true;
                }

                // Obstacle
                else if (fieldDataList[positionIndex.y + dy][positionIndex.x + dx].cellType == 3)
                {
                    observationListNeighborhood[index] = false;
                }

                // Upstair
                else if (fieldDataList[positionIndex.y + dy][positionIndex.x + dx].cellType == 4 && !usedStair)
                {
                    observationListNeighborhood[index] = true;
                }

                // Downstair
                else if (fieldDataList[positionIndex.y + dy][positionIndex.x + dx].cellType == 5 && !usedStair)
                {
                    observationListNeighborhood[index] = true;
                }

                // Otherwise
                else
                {
                    observationListNeighborhood[index] = false;
                }

                index++;
            }
        }
    }


    /// <summary>
    /// Get observation within the range of agentSight
    /// </summary>
    private void GetObservationSquareArea()
    {
        for (int y = -agentSight; y <= agentSight; y++)
        {
            for (int x = -agentSight; x <= agentSight; x++)
            {
                // Out of range
                if (positionIndex.y + y < 0 || positionIndex.y + y >= fieldHeight || positionIndex.x + x < 0 || positionIndex.x + x >= fieldWidth)
                {
                    observationList[y + agentSight][x + agentSight] = 0;
                }

                // Agent itself
                else if (y == 0 && x == 0)
                {
                    observationList[y + agentSight][x + agentSight] = 9;
                }

                // Agent
                else if (fieldAgentData[positionIndex.y + y][positionIndex.x + x])
                {
                    observationList[y + agentSight][x + agentSight] = 10;
                }

                // Empty
                else if (fieldDataList[positionIndex.y + y][positionIndex.x + x].cellType == 1)
                {
                    observationList[y + agentSight][x + agentSight] = 1;
                }

                // Exit
                else if (fieldDataList[positionIndex.y + y][positionIndex.x + x].cellType == 2)
                {
                    observationList[y + agentSight][x + agentSight] = 2;
                }

                // Obstacle
                else if (fieldDataList[positionIndex.y + y][positionIndex.x + x].cellType == 3)
                {
                    observationList[y + agentSight][x + agentSight] = 3;
                }

                // Upstair
                else if (fieldDataList[positionIndex.y + y][positionIndex.x + x].cellType == 4)
                {
                    observationList[y + agentSight][x + agentSight] = 4;
                }

                // Downstair
                else if (fieldDataList[positionIndex.y + y][positionIndex.x + x].cellType == 5)
                {
                    observationList[y + agentSight][x + agentSight] = 5;
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
