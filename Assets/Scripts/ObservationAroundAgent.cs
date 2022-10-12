using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObservationAroundAgent
{
    // Agent's observation
    public List<List<int>> observationList;

    // Agent's observation of around itself (only detecting whether the tile is null or not; the empty tile and exit tile is true)
    public List<bool> observationListNeighborhood;

    // Member var
    private readonly List<List<int>> m_fieldData;
    private readonly List<List<bool>> m_fieldAgentData;
    private readonly int m_fieldHeight;
    private readonly int m_fieldWidth;
    private Vector2Int m_positionIndex;
    private readonly int m_agentSight;


    /// <summary>
    /// Create observation list.
    /// </summary>
    /// <param name="fieldData">fieldData</param>
    /// <param name="fieldAgentData">fieldData of agents</param>
    /// <param name="height">Height of the field</param>
    /// <param name="width">Width of the field</param>
    /// <param name="positionIndex">Index of agent's position index of the fieldData</param>
    /// <param name="agentSight">Agent's sight</param>
    public ObservationAroundAgent(List<List<int>> fieldData, List<List<bool>> fieldAgentData, int height, int width, Vector2Int positionIndex, int agentSight)
    {
        observationList = new();
        observationListNeighborhood = new();
        m_fieldData = fieldData;
        m_fieldAgentData = fieldAgentData;
        m_fieldHeight = height;
        m_fieldWidth = width;
        m_positionIndex = positionIndex;
        m_agentSight = agentSight;

        UpdateObservation(m_positionIndex, agentSight);
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

        m_positionIndex = new_positionIndex;
        GetObservation(sight);
    }


    /// <summary>
    /// Call GetObservation functions.
    /// <param name="sight">Agent's sight</param>
    /// </summary>
    private void GetObservation(int sight)
    {
        GetObservationNeighborhood(m_positionIndex, m_fieldHeight, m_fieldWidth);
        GetObservationSquareArea(m_positionIndex, sight, m_fieldHeight, m_fieldWidth);
        GetObservation_new(m_positionIndex, sight, m_fieldHeight, m_fieldWidth);
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
                else if (m_fieldData[positionIndex.y + y][positionIndex.x + x] == 1)
                {
                    observationListNeighborhood[cnt] = true;
                }

                // Exit
                else if (m_fieldData[positionIndex.y + y][positionIndex.x + x] == 2)
                {
                    observationListNeighborhood[cnt] = true;
                }

                // Obstacle
                else if (m_fieldData[positionIndex.y + y][positionIndex.x + x] == 3)
                {
                    observationListNeighborhood[cnt] = false;
                }

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
                else if (m_fieldAgentData[positionIndex.y + y][positionIndex.x + x])
                {
                    observationList[y + sight][x + sight] = 4;
                }

                // Empty
                else if (m_fieldData[positionIndex.y + y][positionIndex.x + x] == 1)
                {
                    observationList[y + sight][x + sight] = 1;
                }

                // Exit
                else if (m_fieldData[positionIndex.y + y][positionIndex.x + x] == 2)
                {
                    observationList[y + sight][x + sight] = 2;
                }

                // Obstacle
                else if (m_fieldData[positionIndex.y + y][positionIndex.x + x] == 3)
                {
                    observationList[y + sight][x + sight] = 3;
                }
            }
        }
    }


    public void GetObservation_new(Vector2Int positionIndex, int sight, int height, int width)
    {
        for (int y = -sight; y <= sight; y++)
        {
            for (int x = -sight; x <= sight; y++)
            {
                int sx = x + positionIndex.x;
                int sy = y + positionIndex.y;

                // Out of range
                if (sy < 0 || sy >= height || sx < 0 || sx >= width)
                {
                    observationList[sy][sx] = 0;
                    continue;
                }

                // Set diff
                float dx = 0, dy = 0;
                if (Mathf.Abs(sx) > Mathf.Abs(sx))
                {
                    dx = -1 * (sx > 0 ? 1 : -1);
                    dy = -1 / Mathf.Abs(sx) * (sy > 0 ? 1 : -1);
                }
                else
                {
                    dx = -1 / Mathf.Abs(sy) * (sx > 0 ? 1 : -1);
                    dy = -1 * (sy > 0 ? 1 : -1);
                }

                
            }
        }
    }


    public int GetPosData(int x, int y, int height, int width)
    {
        // Out of range
        if (y < 0 || y >= height || x < 0 || x >= width)
        {
            return 0;
        }

        // Agent
        else if (m_fieldAgentData[y][x])
        {
            return 4;
        }

        // Empty
        else if (m_fieldData[y][x] == 1)
        {
            return 1;
        }

        // Exit
        else if (m_fieldData[y][x] == 2)
        {
            return 2;
        }

        // Obstacle
        else if (m_fieldData[y][x] == 3)
        {
            return 3;
        }

        else
        {
            return 0;
        }
    }


    /// <summary>
    /// For debug
    /// </summary>
    /// <param name="agent_id">Agent's id</param>
    public void PrintAgentObservation(int agent_id)
    {
        Debug.Log($"===== Agent{agent_id}'s Observation =====");
        Debug.Log($"[Neighborhood]");
        string s = string.Empty;
        for (int i = 0; i < 9; i++)
        {
            s += observationListNeighborhood[i].ToString() + " ";
        }
        Debug.Log(s);

        Debug.Log($"[Around]");
        for (int y = 0; y < m_agentSight * 2 + 1; y++)
        {
            s = string.Empty;
            for (int x = 0; x < m_agentSight * 2 + 1; x++)
            {
                s += observationList[y][x].ToString() + " ";
            }
            Debug.Log(s);
        }
        Debug.Log("=================================");
    }
}
