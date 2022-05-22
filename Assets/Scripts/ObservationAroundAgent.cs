using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObservationAroundAgent
{
    List<List<int>> m_fieldData;
    public List<List<int>> m_observationList;

    int m_fieldLineSize;
    int m_fieldColumnSize;
    int m_posX;
    int m_posY;
    int m_sight;
    int m_agentCnt;


    public ObservationAroundAgent(List<List<int>> fieldData, int fieldLineSize, int fieldColumnSize, int agentPos_x, int agentPos_y, int agentSight, int agentCnt)
    {
        m_fieldData = fieldData;
        m_observationList = new List<List<int>>();
        m_fieldLineSize = fieldLineSize;
        m_fieldColumnSize = fieldColumnSize;
        m_posX = agentPos_x;
        m_posY = agentPos_y;
        m_sight = agentSight;
        m_agentCnt = agentCnt;

        GetObservation();
    }

    public void PrintAgentObservation(int agent_id)
    {
        Debug.Log($"===== Agent{agent_id}'s Observation =====");
        for (int i = 0; i < m_sight * 2 + 1; i++)
        {
            string s = string.Empty;
            for (int j = 0; j < m_sight * 2 + 1; j++)
            {
                s += m_observationList[i][j].ToString() + " ";
            }
            Debug.Log(s);
        }
        Debug.Log("=================================");
    }

    private void GetObservation()
    {
        for (int i = 0; i < m_sight * 2 + 1; i++)
        {
            List<int> temp = new List<int>();
            for (int j = 0; j < m_sight * 2 + 1; j++)
            {
                temp.Add(0);
            }
            m_observationList.Add(temp);
        }
        m_observationList[m_sight][m_sight] = 9;

        for (int i = 0; i < 4; i++) GetObservationType1(i);
        for (int i = 0; i < 4; i++) GetObservationType2(i);
        for (int i = 0; i < 8; i++) GetObservationType3(i);
    }

    private void GetObservationType1(int dir)
    {
        // Forward
        if (dir == 0)
        {
            bool flag = false;
            for (int i = 1; i <= m_sight; i++)
            {
                // Out of range
                if (m_posY - i < 0)
                {
                    m_observationList[m_sight - i][m_sight] = 0;
                }

                // Empty
                else if (m_fieldData[m_posY - i][m_posX] == 1)
                {
                    if (flag) m_observationList[m_sight - i][m_sight] = 0;
                    else m_observationList[m_sight - i][m_sight] = 1;
                }

                // Exit
                else if (m_fieldData[m_posY - i][m_posX] == 2)
                {
                    if (flag) m_observationList[m_sight - i][m_sight] = 0;
                    else m_observationList[m_sight - i][m_sight] = 2; flag = true;
                    
                }

                // Obstacle
                else if (m_fieldData[m_posY - i][m_posX] == 3)
                {
                    if (flag) m_observationList[m_sight - i][m_sight] = 0;
                    else m_observationList[m_sight - i][m_sight] = 3; flag = true;
                }

                // Agent
                else if (10 <= m_fieldData[m_posY - i][m_posX] && m_fieldData[m_posY - i][m_posX] < 10 + m_agentCnt)
                {
                    if (flag) m_observationList[m_sight - i][m_sight] = 0;
                    else m_observationList[m_sight - i][m_sight] = 4; flag = true;
                }

                // Other
                else
                {
                    m_observationList[m_sight - i][m_sight] = 0;
                }
            }
        }

        // Right
        else if (dir == 1)
        {
            bool flag = false;
            for (int i = 1; i <= m_sight; i++)
            {
                // Out of range
                if (m_fieldColumnSize < m_posX + i)
                {
                    m_observationList[m_sight][m_sight + i] = 0;
                }

                // Empty
                else if (m_fieldData[m_posY][m_posX + i] == 1)
                {
                    if (flag) m_observationList[m_sight][m_sight + i] = 0;
                    else m_observationList[m_sight][m_sight + i] = 1;
                }

                // Exit
                else if (m_fieldData[m_posY][m_posX + i] == 2)
                {
                    if (flag) m_observationList[m_sight][m_sight + i] = 0;
                    else m_observationList[m_sight][m_sight + i] = 2; flag = true;
                }

                // Obstacle
                else if (m_fieldData[m_posY][m_posX + i] == 3)
                {
                    if (flag) m_observationList[m_sight][m_sight + i] = 0;
                    else m_observationList[m_sight][m_sight + i] = 3; flag = true;
                }

                // Agent
                else if (10 <= m_fieldData[m_posY][m_posX + i] && m_fieldData[m_posY][m_posX + i] < 10 + m_agentCnt)
                {
                    if (flag) m_observationList[m_sight][m_sight + i] = 0;
                    else m_observationList[m_sight][m_sight + i] = 4; flag = true;
                }

                // Other
                else
                {
                    m_observationList[m_sight][m_sight + i] = 0;
                }
            }
        }

        // Back
        else if (dir == 2)
        {
            bool flag = false;
            for (int i = 1; i <= m_sight; i++)
            {
                // Out of range
                if (m_fieldLineSize < m_posY + i)
                {
                    m_observationList[m_sight + i][m_sight] = 0;
                }

                // Empty
                else if (m_fieldData[m_posY + i][m_posX] == 1)
                {
                    if (flag) m_observationList[m_sight + i][m_sight] = 0;
                    else m_observationList[m_sight + i][m_sight] = 1;
                }

                // Exit
                else if (m_fieldData[m_posY + i][m_posX] == 2)
                {
                    if (flag) m_observationList[m_sight + i][m_sight] = 0;
                    else m_observationList[m_sight + i][m_sight] = 2; flag = true;
                }

                // Obstacle
                else if (m_fieldData[m_posY + i][m_posX] == 3)
                {
                    if (flag) m_observationList[m_sight + i][m_sight] = 0;
                    else m_observationList[m_sight + i][m_sight] = 3; flag = true;
                }

                // Agent
                else if (10 <= m_fieldData[m_posY + i][m_posX] && m_fieldData[m_posY + i][m_posX] < 10 + m_agentCnt)
                {
                    if (flag) m_observationList[m_sight + i][m_sight] = 0;
                    else m_observationList[m_sight + i][m_sight] = 4; flag = true;
                }

                // Other
                else
                {
                    m_observationList[m_sight + i][m_sight] = 0;
                }
            }
        }

        // Left
        else if (dir == 3)
        {
            bool flag = false;
            for (int i = 1; i <= m_sight; i++)
            {
                // Out of range
                if (m_posX - i < 0)
                {
                    m_observationList[m_sight][m_sight - i] = 0;
                }

                // Empty
                else if (m_fieldData[m_posY][m_posX - i] == 1)
                {
                    if (flag) m_observationList[m_sight][m_sight - i] = 0;
                    else m_observationList[m_sight][m_sight - i] = 1;
                }

                // Exit
                else if (m_fieldData[m_posY][m_posX - i] == 2)
                {
                    if (flag) m_observationList[m_sight][m_sight - i] = 0;
                    else m_observationList[m_sight][m_sight - i] = 2; flag = true;
                }

                // Obstacle
                else if (m_fieldData[m_posY][m_posX - i] == 3)
                {
                    if (flag) m_observationList[m_sight][m_sight - i] = 0;
                    else m_observationList[m_sight][m_sight - i] = 3; flag = true;
                }

                // Agent
                else if (10 <= m_fieldData[m_posY][m_posX - i] && m_fieldData[m_posY][m_posX - i] < 10 + m_agentCnt)
                {
                    if (flag) m_observationList[m_sight][m_sight - i] = 0;
                    else m_observationList[m_sight][m_sight - i] = 4; flag = true;
                }

                // Other
                else
                {
                    m_observationList[m_sight][m_sight - i] = 0;
                }
            }
        }
    }

    private void GetObservationType2(int dir)
    {
        // Right forward
        if (dir == 0)
        {
            bool flag = false;
            for (int i = 1; i <= m_sight; i++)
            {
                // Out of range
                if (m_posY - i < 0 || m_sight * 2 < m_posX + i)
                {
                    m_observationList[m_sight - i][m_sight + i] = 0;
                }

                // Empty
                else if (m_fieldData[m_posY - i][m_posX + i] == 1)
                {
                    if (flag) m_observationList[m_sight - i][m_sight + i] = 0;
                    else m_observationList[m_sight - i][m_sight + i] = 1;
                }

                // Exit
                else if (m_fieldData[m_posY - i][m_posX + i] == 2)
                {
                    if (flag) m_observationList[m_sight - i][m_sight + i] = 0;
                    else m_observationList[m_sight - i][m_sight + i] = 2; flag = true;
                }

                // Obstacle
                else if (m_fieldData[m_posY - i][m_posX + i] == 3)
                {
                    if (flag) m_observationList[m_sight - i][m_sight + i] = 0;
                    else m_observationList[m_sight - i][m_sight + i] = 3; flag = true;
                }

                // Agent
                else if (10 <= m_fieldData[m_posY - i][m_posX + i] && m_fieldData[m_posY - i][m_posX + i] < 10 + m_agentCnt)
                {
                    if (flag) m_observationList[m_sight - i][m_sight + i] = 0;
                    else m_observationList[m_sight - i][m_sight + i] = 4; flag = true;
                }

                // Other
                else
                {
                    m_observationList[m_sight - i][m_sight + i] = 0;
                }
            }
        }

        // Right back
        else if (dir == 1)
        {
            bool flag = false;
            for (int i = 1; i <= m_sight; i++)
            {
                // Out of range
                if (m_fieldLineSize < m_posY + i || m_fieldColumnSize < m_posX + i)
                {
                    m_observationList[m_sight + i][m_sight + i] = 0;
                }

                // Empty
                else if (m_fieldData[m_posY + i][m_posX + i] == 1)
                {
                    if (flag) m_observationList[m_sight + i][m_sight + i] = 0;
                    else m_observationList[m_sight + i][m_sight + i] = 1;
                }

                // Exit
                else if (m_fieldData[m_posY + i][m_posX + i] == 2)
                {
                    if (flag) m_observationList[m_sight + i][m_sight + i] = 0;
                    else m_observationList[m_sight + i][m_sight + i] = 2; flag = true;
                }

                // Obstacle
                else if (m_fieldData[m_posY + i][m_posX + i] == 3)
                {
                    if (flag) m_observationList[m_sight + i][m_sight + i] = 0;
                    else m_observationList[m_sight + i][m_sight + i] = 3; flag = true;
                }

                // Agent
                else if (10 <= m_fieldData[m_posY + i][m_posX + i] && m_fieldData[m_posY + i][m_posX + i] < 10 + m_agentCnt)
                {
                    if (flag) m_observationList[m_sight + i][m_sight + i] = 0;
                    else m_observationList[m_sight + i][m_sight + i] = 4; flag = true;
                }

                // Other
                else
                {
                    m_observationList[m_sight + i][m_sight + i] = 0;
                }
            }
        }

        // Left back
        else if (dir == 2)
        {
            bool flag = false;
            for (int i = 1; i <= m_sight; i++)
            {
                // Out of range
                if (m_fieldLineSize < m_posY + i || m_posX - i < 0)
                {
                    m_observationList[m_sight + i][m_sight - i] = 0;
                }
                
                // Empty
                else if (m_fieldData[m_posY + i][m_posX - i] == 1)
                {
                    if (flag) m_observationList[m_sight + i][m_sight - i] = 0;
                    else m_observationList[m_sight + i][m_sight - i] = 1;
                }

                // Exit
                else if (m_fieldData[m_posY + i][m_posX - i] == 2)
                {
                    if (flag) m_observationList[m_sight + i][m_sight - i] = 0;
                    else m_observationList[m_sight + i][m_sight - i] = 2; flag = true;
                }

                // Obstacle
                else if (m_fieldData[m_posY + i][m_posX - i] == 3)
                {
                    if (flag) m_observationList[m_sight + i][m_sight - i] = 0;
                    else m_observationList[m_sight + i][m_sight - i] = 3; flag = true;
                }

                // Agent
                else if (10 <= m_fieldData[m_posY + i][m_posX - i] && m_fieldData[m_posY + i][m_posX - i] < 10 + m_agentCnt)
                {
                    if (flag) m_observationList[m_sight + i][m_sight - i] = 0;
                    else m_observationList[m_sight + i][m_sight - i] = 4; flag = true;
                }

                // Other
                else
                {
                    m_observationList[m_sight + i][m_sight - i] = 0;
                }
            }
        }

        // Left forward
        else if (dir == 3)
        {
            bool flag = false;
            for (int i = 1; i <= m_sight; i++)
            {
                // Out of range
                if (m_posY - i < 0|| m_posX - i < 0)
                {
                    m_observationList[m_sight - i][m_sight - i] = 0;
                }

                // Empty
                else if (m_fieldData[m_posY - i][m_posX - i] == 1)
                {
                    if (flag) m_observationList[m_sight - i][m_sight - i] = 0;
                    else m_observationList[m_sight - i][m_sight - i] = 1;
                }

                // Exit
                else if (m_fieldData[m_posY - i][m_posX - i] == 2)
                {
                    if (flag) m_observationList[m_sight - i][m_sight - i] = 0;
                    else m_observationList[m_sight - i][m_sight - i] = 2; flag = true;
                }

                // Obstacle
                else if (m_fieldData[m_posY - i][m_posX - i] == 3)
                {
                    if (flag) m_observationList[m_sight - i][m_sight - i] = 0;
                    else m_observationList[m_sight - i][m_sight - i] = 3; flag = true;
                }

                // Agent
                else if (10 <= m_fieldData[m_posY - i][m_posX - i] && m_fieldData[m_posY - i][m_posX - i] < 10 + m_agentCnt)
                {
                    if (flag) m_observationList[m_sight - i][m_sight - i] = 0;
                    else m_observationList[m_sight - i][m_sight - i] = 4; flag = true;
                }

                // Other
                else
                {
                    m_observationList[m_sight - i][m_sight - i] = 0;
                }
            }
        }
    }

    private void GetObservationType3(int dir)
    {
        // Forward right
        if (dir == 0)
        {
            int flag = 0;
            for (int i = 1; i <= m_sight; i++)
            {
                for (int j = 1; j < i; j++)
                {
                    // Out of range
                    if (m_posY - i < 0 || m_fieldColumnSize < m_posX + j)
                    {
                        m_observationList[m_sight - i][m_sight + j] = 0;
                    }

                    // Empty
                    else if (m_fieldData[m_posY - i][m_posX + j] == 1)
                    {
                        if (flag != 0 && flag <= j) m_observationList[m_sight - i][m_sight + j] = 0;
                        else
                        {
                            m_observationList[m_sight - i][m_sight + j] = 1;
                        }
                    }

                    // Exit
                    else if (m_fieldData[m_posY - i][m_posX + j] == 2)
                    {
                        if (flag != 0 && flag <= j) m_observationList[m_sight - i][m_sight + j] = 0;
                        else
                        {
                            m_observationList[m_sight - i][m_sight + j] = 2;
                            flag = j;
                        }
                    }

                    // Obstacle
                    else if (m_fieldData[m_posY - i][m_posX + j] == 3)
                    {
                        if (flag != 0 && flag <= j) m_observationList[m_sight - i][m_sight + j] = 0;
                        else
                        {
                            m_observationList[m_sight - i][m_sight + j] = 3;
                            flag = j;
                        }
                    }

                    // Agent
                    else if (10 <= m_fieldData[m_posY - i][m_posX + j] && m_fieldData[m_posY - i][m_posX + j] < 10 + m_agentCnt)
                    {
                        if (flag != 0 && flag <= j) m_observationList[m_sight - i][m_sight + j] = 0;
                        else
                        {
                            m_observationList[m_sight - i][m_sight + j] = 4;
                            flag = j;
                        }
                    }

                    // Other
                    else
                    {
                        m_observationList[m_sight - i][m_sight + j] = 0;
                    }
                }
            }
        }

        // Right forward
        else if (dir == 1)
        {
            int flag = 0;
            for (int j = 1; j <= m_sight; j++)
            {
                for (int i = 1; i < j; i++)
                {
                    // Out of range
                    if (m_posY - i < 0 || m_fieldColumnSize < m_posX + j)
                    {
                        m_observationList[m_sight - i][m_sight + j] = 0;
                    }

                    // Empty
                    else if (m_fieldData[m_posY - i][m_posX + j] == 1)
                    {
                        if (flag != 0 && flag <= i) m_observationList[m_sight - i][m_sight + j] = 0;
                        else
                        {
                            m_observationList[m_sight - i][m_sight + j] = 1;
                        }
                    }

                    // Exit
                    else if (m_fieldData[m_posY - i][m_posX + j] == 2)
                    {
                        if (flag != 0 && flag <= i) m_observationList[m_sight - i][m_sight + j] = 0;
                        else
                        {
                            m_observationList[m_sight - i][m_sight + j] = 2;
                            flag = i;
                        }
                    }

                    // Obstacle
                    else if (m_fieldData[m_posY - i][m_posX + j] == 3)
                    {
                        if (flag != 0 && flag <= i) m_observationList[m_sight - i][m_sight + j] = 0;
                        else
                        {
                            m_observationList[m_sight - i][m_sight + j] = 3;
                            flag = i;
                        }
                    }

                    // Agent
                    else if (10 <= m_fieldData[m_posY - i][m_posX + j] && m_fieldData[m_posY - i][m_posX + j] < 10 + m_agentCnt)
                    {
                        if (flag != 0 && flag <= i) m_observationList[m_sight - i][m_sight + j] = 0;
                        else
                        {
                            m_observationList[m_sight - i][m_sight + j] = 4;
                            flag = i;
                        }
                    }

                    // Other
                    else
                    {
                        m_observationList[m_sight - i][m_sight + j] = 0;
                    }
                }
            }
        }

        // Right back
        else if (dir == 2)
        {
            int flag = 0;
            for (int j = 1; j <= m_sight; j++)
            {
                for (int i = 1; i < j; i++)
                {
                    // Out of range
                    if (m_fieldLineSize < m_posY + i || m_fieldColumnSize < m_posX + j)
                    {
                        m_observationList[m_sight + i][m_sight + j] = 0;
                    }

                    // Empty
                    else if (m_fieldData[m_posY + i][m_posX + j] == 1)
                    {
                        if (flag != 0 && i <= flag) m_observationList[m_sight + i][m_sight + j] = 0;
                        else
                        {
                            m_observationList[m_sight + i][m_sight + j] = 1;
                        }
                    }

                    // Exit
                    else if (m_fieldData[m_posY + i][m_posX + j] == 2)
                    {
                        if (flag != 0 && i <= flag) m_observationList[m_sight + i][m_sight + j] = 0;
                        else
                        {
                            m_observationList[m_sight + i][m_sight + j] = 2;
                            flag = i;
                        }
                    }

                    // Obstacle
                    else if (m_fieldData[m_posY + i][m_posX + j] == 3)
                    {
                        if (flag != 0 && i <= flag) m_observationList[m_sight + i][m_sight + j] = 0;
                        else
                        {
                            m_observationList[m_sight + i][m_sight + j] = 3;
                            flag = i;
                        }
                    }

                    // Agent
                    else if (10 <= m_fieldData[m_posY + i][m_posX + j] && m_fieldData[m_posY + i][m_posX + j] < 10 + m_agentCnt)
                    {
                        if (flag != 0 && i <= flag) m_observationList[m_sight + i][m_sight + j] = 0;
                        else
                        {
                            m_observationList[m_sight + i][m_sight + j] = 4;
                            flag = i;
                        }
                    }

                    // Other
                    else
                    {
                        m_observationList[m_sight + i][m_sight + j] = 0;
                    }
                }
            }
        }

        // Back right
        else if (dir == 3)
        {
            int flag = 0;
            for (int i = 1; i <= m_sight; i++)
            {
                for (int j = 1; j < i; j++)
                {
                    // Out of range
                    if (m_fieldLineSize < m_posY + i || m_fieldColumnSize < m_posX + j)
                    {
                        m_observationList[m_sight + i][m_sight + j] = 0;
                    }

                    // Empty
                    else if (m_fieldData[m_posY + i][m_posX + j] == 1)
                    {
                        if (flag != 0 && flag <= j) m_observationList[m_sight + i][m_sight + j] = 0;
                        else
                        {
                            m_observationList[m_sight + i][m_sight + j] = 1;
                        }
                    }

                    // Exit
                    else if (m_fieldData[m_posY + i][m_posX + j] == 2)
                    {
                        if (flag != 0 && flag <= j) m_observationList[m_sight + i][m_sight + j] = 0;
                        else
                        {
                            m_observationList[m_sight + i][m_sight + j] = 2;
                            flag = j;
                        }
                    }

                    // Obstacle
                    else if (m_fieldData[m_posY + i][m_posX + j] == 3)
                    {
                        if (flag != 0 && flag <= j) m_observationList[m_sight + i][m_sight + j] = 0;
                        else
                        {
                            m_observationList[m_sight + i][m_sight + j] = 3;
                            flag = j;
                        }
                    }

                    // Agent
                    else if (10 <= m_fieldData[m_posY + i][m_posX + j] && m_fieldData[m_posY + i][m_posX + j] < 10 + m_agentCnt)
                    {
                        if (flag != 0 && flag <= j) m_observationList[m_sight + i][m_sight + j] = 0;
                        else
                        {
                            m_observationList[m_sight + i][m_sight + j] = 4;
                            flag = j;
                        }
                    }

                    // Other
                    else
                    {
                        m_observationList[m_sight + i][m_sight + j] = 0;
                    }
                }
            }
        }

        // Back left
        else if (dir == 4)
        {
            int flag = 0;
            for (int i = 1; i <= m_sight; i++)
            {
                for (int j = 1; j < i; j++)
                {
                    // Out of range
                    if (m_fieldLineSize < m_posY + i || m_posX - j < 0)
                    {
                        m_observationList[m_sight + i][m_sight - j] = 0;
                    }

                    // Empty
                    else if (m_fieldData[m_posY + i][m_posX - j] == 1)
                    {
                        if (flag != 0 && flag <= j) m_observationList[m_sight + i][m_sight - j] = 0;
                        else
                        {
                            m_observationList[m_sight + i][m_sight - j] = 1;
                        }
                    }

                    // Exit
                    else if (m_fieldData[m_posY + i][m_posX - j] == 2)
                    {
                        if (flag != 0 && flag <= j) m_observationList[m_sight + i][m_sight - j] = 0;
                        else
                        {
                            m_observationList[m_sight + i][m_sight - j] = 2;
                            flag = j;
                        }
                    }

                    // Obstacle
                    else if (m_fieldData[m_posY + i][m_posX - j] == 3)
                    {
                        if (flag != 0 && flag <= j) m_observationList[m_sight + i][m_sight - j] = 0;
                        else
                        {
                            m_observationList[m_sight + i][m_sight - j] = 3;
                            flag = j;
                        }
                    }

                    // Agent
                    else if (10 <= m_fieldData[m_posY + i][m_posX - j] && m_fieldData[m_posY + i][m_posX - j] < 10 + m_agentCnt)
                    {
                        if (flag != 0 && flag <= j) m_observationList[m_sight + i][m_sight - j] = 0;
                        else
                        {
                            m_observationList[m_sight + i][m_sight - j] = 4;
                            flag = j;
                        }
                    }

                    // Other
                    else
                    {
                        m_observationList[m_sight + i][m_sight - j] = 0;
                    }
                }
            }
        }

        // Left back
        else if (dir == 5)
        {
            int flag = 0;
            for (int j = 1; j <= m_sight; j++)
            {
                for (int i = 1; i < j; i++)
                {
                    // Out of range
                    if (m_fieldLineSize < m_posY + i || m_posX - j < 0)
                    {
                        m_observationList[m_sight + i][m_sight - j] = 0;
                    }

                    // Empty
                    else if (m_fieldData[m_posY + i][m_posX - j] == 1)
                    {
                        if (flag != 0 && flag <= i) m_observationList[m_sight + i][m_sight - j] = 0;
                        else
                        {
                            m_observationList[m_sight + i][m_sight - j] = 1;
                        }
                    }

                    // Exit
                    else if (m_fieldData[m_posY + i][m_posX - j] == 2)
                    {
                        if (flag != 0 && flag <= i) m_observationList[m_sight + i][m_sight - j] = 0;
                        else
                        {
                            m_observationList[m_sight + i][m_sight - j] = 2;
                            flag = i;
                        }
                    }

                    // Obstacle
                    else if (m_fieldData[m_posY + i][m_posX - j] == 3)
                    {
                        if (flag != 0 && flag <= i) m_observationList[m_sight + i][m_sight - j] = 0;
                        else
                        {
                            m_observationList[m_sight + i][m_sight - j] = 3;
                            flag = i;
                        }
                    }

                    // Agent
                    else if (10 <= m_fieldData[m_posY + i][m_posX - j] && m_fieldData[m_posY + i][m_posX - j] < 10 + m_agentCnt)
                    {
                        if (flag != 0 && flag <= i) m_observationList[m_sight + i][m_sight - j] = 0;
                        else
                        {
                            m_observationList[m_sight + i][m_sight - j] = 4;
                            flag = i;
                        }
                    }

                    // Other
                    else
                    {
                        m_observationList[m_sight + i][m_sight - j] = 0;
                    }
                }
            }
        }

        // Left forward
        else if (dir == 6)
        {
            int flag = 0;
            for (int j = 1; j <= m_sight; j++)
            {
                for (int i = 1; i < j; i++)
                {
                    // Out of range
                    if (m_posY - i < 0 || m_posX - j < 0)
                    {
                        m_observationList[m_sight - i][m_sight - j] = 0;
                    }

                    // Empty
                    else if (m_fieldData[m_posY - i][m_posX - j] == 1)
                    {
                        if (flag != 0 && flag <= i) m_observationList[m_sight - i][m_sight - j] = 0;
                        else
                        {
                            m_observationList[m_sight - i][m_sight - j] = i;
                        }
                    }

                    // Exit
                    else if (m_fieldData[m_posY - i][m_posX - j] == 2)
                    {
                        if (flag != 0 && flag <= i) m_observationList[m_sight - i][m_sight - j] = 0;
                        else
                        {
                            m_observationList[m_sight - i][m_sight - j] = 2;
                            flag = i;
                        }
                    }

                    // Obstacle
                    else if (m_fieldData[m_posY - i][m_posX - j] == 3)
                    {
                        if (flag != 0 && flag <= i) m_observationList[m_sight - i][m_sight - j] = 0;
                        else
                        {
                            m_observationList[m_sight - i][m_sight - j] = 3;
                            flag = i;
                        }
                    }

                    // Agent
                    else if (10 <= m_fieldData[m_posY - i][m_posX - j] && m_fieldData[m_posY - i][m_posX - j] < 10 + m_agentCnt)
                    {
                        if (flag != 0 && flag <= i) m_observationList[m_sight - i][m_sight - j] = 0;
                        else
                        {
                            m_observationList[m_sight - i][m_sight - j] = 4;
                            flag = i;
                        }
                    }

                    // Other
                    else
                    {
                        m_observationList[m_sight - i][m_sight - j] = 0;
                    }
                }
            }
        }

        // Forward left
        else if (dir == 7)
        {
            int flag = 0;
            for (int i = 1; i <= m_sight; i++)
            {
                if (flag != 0) flag++;

                for (int j = 1; j < i; j++)
                {
                    // Out of range
                    if (m_posY - i < 0 || m_posX - j < 0)
                    {
                        m_observationList[m_sight - i][m_sight - j] = 0;
                    }

                    // Empty
                    else if (m_fieldData[m_posY - i][m_posX - j] == 1)
                    {
                        if (flag != 0 && flag <= j) m_observationList[m_sight - i][m_sight - j] = 0;
                        else
                        {
                            m_observationList[m_sight - i][m_sight - j] = 1;
                        }
                    }

                    // Exit
                    else if (m_fieldData[m_posY - i][m_posX - j] == 2)
                    {
                        if (flag != 0 && j <= flag) m_observationList[m_sight - i][m_sight - j] = 0;
                        else
                        {
                            m_observationList[m_sight - i][m_sight - j] = 2;
                            flag = j;
                        }
                    }

                    // Obstacle
                    else if (m_fieldData[m_posY - i][m_posX - j] == 3)
                    {
                        if (flag != 0 && j <= flag) m_observationList[m_sight - i][m_sight - j] = 0;
                        else
                        {
                            m_observationList[m_sight - i][m_sight - j] = 3;
                            flag = j;
                        }
                    }

                    // Agent
                    else if (10 <= m_fieldData[m_posY - i][m_posX - j] && m_fieldData[m_posY - i][m_posX - j] < 10 + m_agentCnt)
                    {
                        if (flag != 0 && j <= flag) m_observationList[m_sight - i][m_sight - j] = 0;
                        else
                        {
                            m_observationList[m_sight - i][m_sight - j] = 4;
                            flag = j;
                        }
                    }

                    // Other
                    else
                    {
                        m_observationList[m_sight - i][m_sight - j] = 0;
                    }
                }
            }
        }
    }
}