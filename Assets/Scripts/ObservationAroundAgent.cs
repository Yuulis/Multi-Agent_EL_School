using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObservationAroundAgent
{
    // Agent's observation
    public List<List<int>> observationList;

    // Agent's observation of around it (only detecting whether the tile is empty or not; the empty tile and exit tile is true)
    public List<bool> observationListNeighborhood;

    // Member var
    private readonly List<List<TileCellInfo>> m_fieldData2D;
    private readonly int m_fieldHeight;
    private readonly int m_fieldWidth;
    int m_agentIndex_X;
    int m_agentIndex_Y;
    private readonly int m_agentSight;
    private readonly int m_agentCnt;


    /// <summary>
    /// Create observation list.
    /// </summary>
    /// <param name="fieldData2D"></param>
    /// <param name="height">height of field</param>
    /// <param name="width">width of field</param>
    /// <param name="agentIndex_x">Second-index of the agent's position of the fieldData2D</param>
    /// <param name="agentIndex_y">first-index of the agent's position of the fieldData2D</param>
    /// <param name="agentSight">Agent's sight</param>
    /// <param name="agentCnt">Number of Agents</param>
    public ObservationAroundAgent(List<List<TileCellInfo>> fieldData2D, int height, int width, int agentIndex_x, int agentIndex_y, int agentSight, int agentCnt)
    {
        observationList = new();
        observationListNeighborhood = new();
        m_fieldData2D = fieldData2D;
        m_fieldHeight = height;
        m_fieldWidth = width;
        m_agentIndex_X = agentIndex_x;
        m_agentIndex_Y = agentIndex_y;
        m_agentSight = agentSight;
        m_agentCnt = agentCnt;

        UpdateObservation(agentIndex_x, agentIndex_y);
    }


    /// <summary>
    /// Update observation list.
    /// </summary>
    /// <param name="new_agentIndex_x">Second-index of the agent's new position of the fieldData2D<</param>
    /// <param name="new_agentIndex_y">first-index of the agent's new position of the fieldData2D</param>
    public void UpdateObservation(int new_agentIndex_x, int new_agentIndex_y)
    {
        observationList.Clear();
        observationListNeighborhood.Clear();

        for (int i = 0; i < 10; i++)
        {
            observationListNeighborhood.Add(false);
        }

        m_agentIndex_X = new_agentIndex_x;
        m_agentIndex_Y = new_agentIndex_y;

        GetObservation(m_agentSight);
    }


    /// <summary>
    /// Initialize observation list and call four GetObservation functions.
    /// </summary>
    private void GetObservation(int sight)
    {
        for (int y = 0; y < sight * 2 + 1; y++)
        {
            List<int> temp = new();
            for (int x = 0; x < sight * 2 + 1; x++)
            {
                temp.Add(0);
            }
            observationList.Add(temp);
        }

        GetObservationSquareArea(m_agentIndex_X, m_agentIndex_Y, sight, m_fieldHeight, m_fieldWidth);
    }

    /// <summary>
    /// Get observation of Agent'S neighborhood(nine tiles)
    /// </summary>
    public void GetObservationNeighborhood(int idx_x, int idx_y)
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
                else if (m_fieldData2D[idx_y + y][idx_x + x].tileNum == 1)
                {
                    observationListNeighborhood[cnt] = true;
                }

                // Exit
                else if (m_fieldData2D[idx_y + y][idx_x + x].tileNum == 2)
                {
                    observationListNeighborhood[cnt] = true;
                }

                // Obstacle
                else if (m_fieldData2D[idx_y + y][idx_x + x].tileNum == 3)
                {
                    observationListNeighborhood[cnt] = false;
                }

                // Agent
                else if (m_fieldData2D[idx_y + y][idx_x + x].tileNum == 4)
                {
                    observationListNeighborhood[cnt] = false;
                }

                cnt++;
            }
        }
    }


    private void GetObservationSquareArea(int idx_x, int idx_y, int sight, int height, int width)
    {
        for (int y = -sight; y < sight; y++)
        {
            for (int x = -sight; x < sight; x++)
            {
                // Out of range
                if (idx_y + y < 0 || idx_y + y >= height || idx_x + x < 0 || idx_x + x >= width)
                {
                    observationList[y + sight][x + sight] = 0;
                }

                // Agent itself
                if (y == 0 && x == 0)
                {
                    observationList[y + sight][x + sight] = 9;
                }

                // Empty
                else if (m_fieldData2D[idx_y + y][idx_x + x].tileNum == 1)
                {
                    observationList[y + sight][x + sight] = 1;
                }

                // Exit
                else if (m_fieldData2D[idx_y + y][idx_x + x].tileNum == 2)
                {
                    observationList[y + sight][x + sight] = 2;
                }

                // Obstacle
                else if (m_fieldData2D[idx_y + y][idx_x + x].tileNum == 3)
                {
                    observationList[y + sight][x + sight] = 3;
                }

                // Agent
                else if (10 <= m_fieldData2D[idx_y + y][idx_x + x].tileNum && m_fieldData2D[idx_y + y][idx_x + x].tileNum < 10 + m_agentCnt)
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
        for (int i = 0; i < m_agentSight * 2 + 1; i++)
        {
            string s = string.Empty;
            for (int j = 0; j < m_agentSight * 2 + 1; j++)
            {
                s += observationList[i][j].ToString() + " ";
            }
            Debug.Log(s);
        }
        Debug.Log("=================================");
    }
}
