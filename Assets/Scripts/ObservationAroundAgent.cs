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
    private readonly List<List<int>> m_fieldData2D;
    private readonly int m_fieldHeight;
    private readonly int m_fieldWidth;
    private Vector2Int m_positionIndex;
    private readonly int m_agentSight;
    private readonly int m_agentCnt;


    /// <summary>
    /// Create observation list.
    /// </summary>
    /// <param name="fieldData"></param>
    /// <param name="height">Height of the field</param>
    /// <param name="width">Width of the field</param>
    /// <param name="positionIndex">Index of agent's position index of the fieldData</param>
    /// <param name="agentSight">Agent's sight</param>
    /// <param name="agentCnt">Number of Agents</param>
    public ObservationAroundAgent(List<List<int>> fieldData, int height, int width, Vector2Int positionIndex, int agentSight, int agentCnt)
    {
        observationList = new();
        observationListNeighborhood = new();
        m_fieldData2D = fieldData;
        m_fieldHeight = height;
        m_fieldWidth = width;
        m_positionIndex = positionIndex;
        m_agentSight = agentSight;
        m_agentCnt = agentCnt;

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
        for (int i = 0; i < 10; i++)
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
        GetObservationNeighborhood(m_positionIndex);
        GetObservationSquareArea(m_positionIndex, sight, m_fieldHeight, m_fieldWidth);
    }

    /// <summary>
    /// Get observation of Agent'S neighborhood(nine tiles)
    /// <param name="positionIndex">Index of agent's position index of the fieldData</param>
    /// </summary>
    public void GetObservationNeighborhood(Vector2Int positionIndex)
    {
        int cnt = 0;
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                // Agent itself
                if (y == 0 && x == 0)
                {
                    observationListNeighborhood[cnt] = false;
                }

                // Empty
                else if (m_fieldData2D[positionIndex.y + y][positionIndex.x + x] == 1)
                {
                    observationListNeighborhood[cnt] = true;
                }

                // Exit
                else if (m_fieldData2D[positionIndex.y + y][positionIndex.x + x] == 2)
                {
                    observationListNeighborhood[cnt] = true;
                }

                // Obstacle
                else if (m_fieldData2D[positionIndex.y + y][positionIndex.x + x] == 3)
                {
                    observationListNeighborhood[cnt] = false;
                }

                // Agent
                else if (m_fieldData2D[positionIndex.y + y][positionIndex.x + x] == 4)
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
        for (int y = -sight; y < sight; y++)
        {
            for (int x = -sight; x < sight; x++)
            {
                // Out of range
                if (positionIndex.y + y < 0 || positionIndex.y + y >= height || positionIndex.x + x < 0 || positionIndex.x + x >= width)
                {
                    observationList[y + sight][x + sight] = 0;
                }

                // Agent itself
                if (y == 0 && x == 0)
                {
                    observationList[y + sight][x + sight] = 9;
                }

                // Empty
                else if (m_fieldData2D[positionIndex.y + y][positionIndex.x + x] == 1)
                {
                    observationList[y + sight][x + sight] = 1;
                }

                // Exit
                else if (m_fieldData2D[positionIndex.y + y][positionIndex.x + x] == 2)
                {
                    observationList[y + sight][x + sight] = 2;
                }

                // Obstacle
                else if (m_fieldData2D[positionIndex.y + y][positionIndex.x + x] == 3)
                {
                    observationList[y + sight][x + sight] = 3;
                }

                // Agent
                else if (10 <= m_fieldData2D[positionIndex.y + y][positionIndex.x + x] && m_fieldData2D[positionIndex.y + y][positionIndex.x + x] < 10 + m_agentCnt)
                {
                    observationList[y + sight][x + sight] = 4;
                }
            }
        }
    }


    /// <summary>
    /// For debug
    /// </summary>
    /// <param name="agent_id">Agent's id</param>
    public void PrintAgentObservation(int agent_id)
    {
        Debug.Log($"===== Agent{agent_id}'s Observation =====");
        for (int y = 0; y < m_agentSight * 2 + 1; y++)
        {
            string s = string.Empty;
            for (int x = 0; x < m_agentSight * 2 + 1; x++)
            {
                s += observationList[y][x].ToString() + " ";
            }
            Debug.Log(s);
        }
        Debug.Log("=================================");
    }
}
