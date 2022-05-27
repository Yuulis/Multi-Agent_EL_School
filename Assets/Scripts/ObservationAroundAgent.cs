using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObservationAroundAgent
{
    public List<List<int>> observationList;
    private int[] observationNeighborhood;

    List<List<int>> m_fieldData;
    int m_fieldLineSize;
    int m_fieldColumnSize;
    int m_posX;
    int m_posY;
    int m_sight;
    int m_agentCnt;


    public ObservationAroundAgent(List<List<int>> fieldData, int fieldLineSize, int fieldColumnSize, int agentPos_x, int agentPos_y, int agentSight, int agentCnt)
    {
        observationList = new List<List<int>>();
        observationNeighborhood = new int[10];
        m_fieldData = fieldData;
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
                s += observationList[i][j].ToString() + " ";
            }
            Debug.Log(s);
        }
        Debug.Log("=================================");
    }

    private void GetObservation()
    {
        for (int i = 0; i < m_sight * 2 + 1; i++)
        {
            List<int> temp = new();
            for (int j = 0; j < m_sight * 2 + 1; j++)
            {
                temp.Add(0);
            }
            observationList.Add(temp);
        }

        GetObservationNeighborhood();
        for (int i = 0; i < 4; i++) GetObservationStraight(i);
        for (int i = 0; i < 4; i++) GetObservationDiagonal(i);
        for (int i = 0; i < 8; i++) GetObservationSubDiagonal(i);
    }

    private void GetObservationNeighborhood()
    {
        int cnt = 0;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                // Agent itself
                if (i == 0 && j == 0)
                {
                    observationNeighborhood[cnt] = 9;
                    observationList[m_sight + i][m_sight + j] = 9;
                }

                // Empty
                else if (m_fieldData[m_posY + i][m_posX + j] == 1)
                {
                    observationNeighborhood[cnt] = 1;
                    observationList[m_sight + i][m_sight + j] = 1;
                }

                // Exit
                else if (m_fieldData[m_posY + i][m_posX + j] == 2)
                {
                    observationNeighborhood[cnt] = 2;
                    observationList[m_sight + i][m_sight + j] = 2;
                }

                // Obstacle
                else if (m_fieldData[m_posY + i][m_posX + j] == 3)
                {
                    observationNeighborhood[cnt] = 3;
                    observationList[m_sight + i][m_sight + j] = 3;
                }

                // Agent
                else if (m_fieldData[m_posY + i][m_posX + j] == 4)
                {
                    observationNeighborhood[cnt] = 4;
                    observationList[m_sight + i][m_sight + j] = 4;
                }

                cnt++;
            }
        }
    }

    private void GetObservationStraight(int dir)
    {
        // Forward
        if (dir == 0)
        {
            // Neighboorhood is not empty
            if (observationNeighborhood[1] != 1) return;

            for (int i = 2; i <= m_sight; i++)
            {
                // Out of range
                if (m_posY - i < 0) return;

                // Empty
                else if (m_fieldData[m_posY - i][m_posX] == 1)
                {
                    observationList[m_sight - i][m_sight] = 1;
                }

                // Exit
                else if (m_fieldData[m_posY - i][m_posX] == 2)
                {
                    observationList[m_sight - i][m_sight] = 2;
                    return;
                    
                }

                // Obstacle
                else if (m_fieldData[m_posY - i][m_posX] == 3)
                {
                    observationList[m_sight - i][m_sight] = 3;
                    return;
                }

                // Agent
                else if (10 <= m_fieldData[m_posY - i][m_posX] && m_fieldData[m_posY - i][m_posX] < 10 + m_agentCnt)
                {
                    observationList[m_sight - i][m_sight] = 4;
                    return;
                }
            }
        }

        // Right
        else if (dir == 1)
        {
            // Neighboorhood is not empty
            if (observationNeighborhood[5] != 1) return;

            for (int i = 2; i <= m_sight; i++)
            {
                // Out of range
                if (m_fieldColumnSize < m_posX + i) return;

                // Empty
                else if (m_fieldData[m_posY][m_posX + i] == 1)
                {
                    observationList[m_sight][m_sight + i] = 1;
                }

                // Exit
                else if (m_fieldData[m_posY][m_posX + i] == 2)
                {
                    observationList[m_sight][m_sight + i] = 2;
                    return;
                }

                // Obstacle
                else if (m_fieldData[m_posY][m_posX + i] == 3)
                {
                    observationList[m_sight][m_sight + i] = 3;
                    return;
                }

                // Agent
                else if (10 <= m_fieldData[m_posY][m_posX + i] && m_fieldData[m_posY][m_posX + i] < 10 + m_agentCnt)
                {
                    observationList[m_sight][m_sight + i] = 4;
                    return;
                }
            }
        }

        // Back
        else if (dir == 2)
        {
            // Neighboorhood is not empty
            if (observationNeighborhood[7] != 1) return;

            for (int i = 2; i <= m_sight; i++)
            {
                // Out of range
                if (m_fieldLineSize < m_posY + i) return;

                // Empty
                else if (m_fieldData[m_posY + i][m_posX] == 1)
                {
                    observationList[m_sight + i][m_sight] = 1;
                }

                // Exit
                else if (m_fieldData[m_posY + i][m_posX] == 2)
                {
                    observationList[m_sight + i][m_sight] = 2;
                    return;
                }

                // Obstacle
                else if (m_fieldData[m_posY + i][m_posX] == 3)
                {
                    observationList[m_sight + i][m_sight] = 3;
                    return;
                }

                // Agent
                else if (10 <= m_fieldData[m_posY + i][m_posX] && m_fieldData[m_posY + i][m_posX] < 10 + m_agentCnt)
                {
                    observationList[m_sight + i][m_sight] = 4;
                    return;
                }
            }
        }

        // Left
        else if (dir == 3)
        {
            // Neighboorhood is not empty
            if (observationNeighborhood[3] != 1) return;

            for (int i = 2; i <= m_sight; i++)
            {
                // Out of range
                if (m_posX - i < 0) return;

                // Empty
                else if (m_fieldData[m_posY][m_posX - i] == 1)
                {
                    observationList[m_sight][m_sight - i] = 1;
                }

                // Exit
                else if (m_fieldData[m_posY][m_posX - i] == 2)
                {
                    observationList[m_sight][m_sight - i] = 2;
                    return;
                }

                // Obstacle
                else if (m_fieldData[m_posY][m_posX - i] == 3)
                {
                   observationList[m_sight][m_sight - i] = 3;
                    return;
                }

                // Agent
                else if (10 <= m_fieldData[m_posY][m_posX - i] && m_fieldData[m_posY][m_posX - i] < 10 + m_agentCnt)
                {
                    observationList[m_sight][m_sight - i] = 4;
                    return;
                }
            }
        }
    }

    private void GetObservationDiagonal(int dir)
    {
        // Right forward
        if (dir == 0)
        {
            // Neighboorhood is not empty
            if (observationNeighborhood[2] != 1) return;

            for (int i = 2; i <= m_sight; i++)
            {
                // Out of range
                if (m_posY - i < 0 || m_fieldColumnSize < m_posX + i) return;

                // Empty
                else if (m_fieldData[m_posY - i][m_posX + i] == 1)
                {
                    observationList[m_sight - i][m_sight + i] = 1;
                }

                // Exit
                else if (m_fieldData[m_posY - i][m_posX + i] == 2)
                {
                    observationList[m_sight - i][m_sight + i] = 2;
                    return;
                }

                // Obstacle
                else if (m_fieldData[m_posY - i][m_posX + i] == 3)
                {
                    observationList[m_sight - i][m_sight + i] = 3;
                    return;
                }

                // Agent
                else if (10 <= m_fieldData[m_posY - i][m_posX + i] && m_fieldData[m_posY - i][m_posX + i] < 10 + m_agentCnt)
                {
                    observationList[m_sight - i][m_sight + i] = 4;
                    return;
                }
            }
        }

        // Right back
        else if (dir == 1)
        {
            // Neighboorhood is not empty
            if (observationNeighborhood[8] != 1) return;

            for (int i = 2; i <= m_sight; i++)
            {
                // Out of range
                if (m_fieldLineSize < m_posY + i || m_fieldColumnSize < m_posX + i) return;

                // Empty
                else if (m_fieldData[m_posY + i][m_posX + i] == 1)
                {
                    observationList[m_sight + i][m_sight + i] = 1;
                }

                // Exit
                else if (m_fieldData[m_posY + i][m_posX + i] == 2)
                {
                    observationList[m_sight + i][m_sight + i] = 2;
                    return;
                }

                // Obstacle
                else if (m_fieldData[m_posY + i][m_posX + i] == 3)
                {
                    observationList[m_sight + i][m_sight + i] = 3;
                    return;
                }

                // Agent
                else if (10 <= m_fieldData[m_posY + i][m_posX + i] && m_fieldData[m_posY + i][m_posX + i] < 10 + m_agentCnt)
                {
                    observationList[m_sight + i][m_sight + i] = 4;
                    return;
                }
            }
        }

        // Left back
        else if (dir == 2)
        {
            // Neighboorhood is not empty
            if (observationNeighborhood[6] != 1) return;

            for (int i = 2; i <= m_sight; i++)
            {
                // Out of range
                if (m_fieldLineSize < m_posY + i || m_posX - i < 0) return;

                // Empty
                else if (m_fieldData[m_posY + i][m_posX - i] == 1)
                {
                    observationList[m_sight + i][m_sight - i] = 1;
                }

                // Exit
                else if (m_fieldData[m_posY + i][m_posX - i] == 2)
                {
                    observationList[m_sight + i][m_sight - i] = 2;
                    return;
                }

                // Obstacle
                else if (m_fieldData[m_posY + i][m_posX - i] == 3)
                {
                    observationList[m_sight + i][m_sight - i] = 3;
                    return;
                }

                // Agent
                else if (10 <= m_fieldData[m_posY + i][m_posX - i] && m_fieldData[m_posY + i][m_posX - i] < 10 + m_agentCnt)
                {
                    observationList[m_sight + i][m_sight - i] = 4;
                    return;
                }
            }
        }

        // Left forward
        else if (dir == 3)
        {
            // Neighboorhood is not empty
            if (observationNeighborhood[0] != 1) return;

            for (int i = 2; i <= m_sight; i++)
            {
                // Out of range
                if (m_posY - i < 0 || m_posX - i < 0) return;

                // Empty
                else if (m_fieldData[m_posY - i][m_posX - i] == 1)
                {
                    observationList[m_sight - i][m_sight - i] = 1;
                }

                // Exit
                else if (m_fieldData[m_posY - i][m_posX - i] == 2)
                {
                    observationList[m_sight - i][m_sight - i] = 2;
                    return;
                }

                // Obstacle
                else if (m_fieldData[m_posY - i][m_posX - i] == 3)
                {
                    observationList[m_sight - i][m_sight - i] = 3;
                    return;
                }

                // Agent
                else if (10 <= m_fieldData[m_posY - i][m_posX - i] && m_fieldData[m_posY - i][m_posX - i] < 10 + m_agentCnt)
                {
                    observationList[m_sight - i][m_sight - i] = 4;
                    return;
                }
            }
        }
    }

    private void GetObservationSubDiagonal(int dir)
    {
        // Forward right
        if (dir == 0)
        {
            // Neighboorhood is not empty
            if (observationNeighborhood[1] != 1) return;

            int fop = -1;  // First obstacle pos
            int mop = -1;  // Max obstacle pos
            bool skip = false;

            for (int i = 2; i <= m_sight; i++)
            {
                if (fop != -1 && mop == -1) mop = fop;

                if (mop != -1)
                {
                    mop++;
                    skip = true;
                }

                for (int j = 0; j < i; j++)
                {
                    // Out of range
                    if (m_posY - i < 0 || m_fieldColumnSize < m_posX + j) return;

                    // Cannot be seen from Agent
                    if (fop <= j && j <= mop)
                    {
                        continue;
                    }

                    // Can be seen from Agent
                    else
                    {
                        skip = false;
                    }

                    // Empty
                    if (m_fieldData[m_posY - i][m_posX + j] == 1)
                    {
                        if (j != 0) observationList[m_sight - i][m_sight + j] = 1;
                    }

                    // Exit
                    else if (m_fieldData[m_posY - i][m_posX + j] == 2)
                    {
                        if (j != 0) observationList[m_sight - i][m_sight + j] = 2;
                        if (!skip)
                        {
                            if (fop == -1) fop = j;
                            else mop = j;
                        }
                    }

                    // Obstacle
                    else if (m_fieldData[m_posY - i][m_posX + j] == 3)
                    {
                        if (j != 0) observationList[m_sight - i][m_sight + j] = 3;
                        if (!skip)
                        {
                            if (fop == -1) fop = j;
                            else mop = j;
                        }
                    }

                    // Agent
                    else if (10 <= m_fieldData[m_posY - i][m_posX + j] && m_fieldData[m_posY - i][m_posX + j] < 10 + m_agentCnt)
                    {
                        if (j != 0) observationList[m_sight - i][m_sight + j] = 4;
                        if (!skip)
                        {
                            if (fop == -1) fop = j;
                            else mop = j;
                        }
                    }
                }
            }
        }

        // Right forward
        else if (dir == 1)
        {
            // Neighboorhood is not empty
            if (observationNeighborhood[5] != 1) return;

            int fop = -1;  // First obstacle pos
            int mop = -1;  // Max obstacle pos
            bool skip = false;

            for (int j = 2; j <= m_sight; j++)
            {
                if (fop != -1 && mop == -1) mop = fop;

                if (mop != -1)
                {
                    mop++;
                    skip = true;
                }

                for (int i = 0; i < j; i++)
                {
                    // Out of range
                    if (m_posY - i < 0 || m_fieldColumnSize < m_posX + j) return;

                    // Cannot be seen from Agent
                    if (fop <= i && i <= mop)
                    {
                        continue;
                    }

                    // Can be seen from Agent
                    else
                    {
                        skip = false;
                    }

                    // Empty
                    if (m_fieldData[m_posY - i][m_posX + j] == 1)
                    {
                        if (i != 0) observationList[m_sight - i][m_sight + j] = 1;
                    }

                    // Exit
                    else if (m_fieldData[m_posY - i][m_posX + j] == 2)
                    {
                        if (i != 0) observationList[m_sight - i][m_sight + j] = 2;
                        if (!skip)
                        {
                            if (fop == -1) fop = i;
                            else mop = i;
                        }
                    }

                    // Obstacle
                    else if (m_fieldData[m_posY - i][m_posX + j] == 3)
                    {
                        if (i != 0) observationList[m_sight - i][m_sight + j] = 3;
                        if (!skip)
                        {
                            if (fop == -1) fop = j;
                            else mop = j;
                        }
                    }

                    // Agent
                    else if (10 <= m_fieldData[m_posY - i][m_posX + j] && m_fieldData[m_posY - i][m_posX + j] < 10 + m_agentCnt)
                    {
                        if (i != 0) observationList[m_sight - i][m_sight + j] = 4;
                        if (!skip)
                        {
                            if (fop == -1) fop = j;
                            else mop = j;
                        }
                    }
                }
            }
        }

        // Right back
        else if (dir == 2)
        {
            // Neighboorhood is not empty
            if (observationNeighborhood[5] != 1) return;

            int fop = -1;  // First obstacle pos
            int mop = -1;  // Max obstacle pos
            bool skip = false;

            for (int j = 2; j <= m_sight; j++)
            {
                if (fop != -1 && mop == -1) mop = fop;

                if (mop != -1)
                {
                    mop++;
                    skip = true;
                }

                for (int i = 0; i < j; i++)
                {
                    // Out of range
                    if (m_fieldLineSize < m_posY + i || m_fieldColumnSize < m_posX + j) return;

                    // Cannot be seen from Agent
                    if (fop <= i && i <= mop)
                    {
                        continue;
                    }

                    // Can be seen from Agent
                    else
                    {
                        skip = false;
                    }

                    // Empty
                    if (m_fieldData[m_posY + i][m_posX + j] == 1)
                    {
                        if (i != 0) observationList[m_sight + i][m_sight + j] = 1;
                    }

                    // Exit
                    else if (m_fieldData[m_posY + i][m_posX + j] == 2)
                    {
                        if (i != 0) observationList[m_sight + i][m_sight + j] = 2;
                        if (!skip)
                        {
                            if (fop == -1) fop = i;
                            else mop = i;
                        }
                    }

                    // Obstacle
                    else if (m_fieldData[m_posY + i][m_posX + j] == 3)
                    {
                        if (i != 0) observationList[m_sight + i][m_sight + j] = 3;
                        if (!skip)
                        {
                            if (fop == -1) fop = i;
                            else mop = i;
                        }
                    }

                    // Agent
                    else if (10 <= m_fieldData[m_posY + i][m_posX + j] && m_fieldData[m_posY + i][m_posX + j] < 10 + m_agentCnt)
                    {
                        if (i != 0) observationList[m_sight + i][m_sight + j] = 4;
                        if (!skip)
                        {
                            if (fop == -1) fop = i;
                            else mop = i;
                        }
                    }
                }
            }
        }

        // Back right
        else if (dir == 3)
        {
            // Neighboorhood is not empty
            if (observationNeighborhood[7] != 1) return;

            int fop = -1;  // First obstacle pos
            int mop = -1;  // Max obstacle pos
            bool skip = false;

            for (int i = 2; i <= m_sight; i++)
            {
                if (fop != -1 && mop == -1) mop = fop;

                if (mop != -1)
                {
                    mop++;
                    skip = true;
                }

                for (int j = 0; j < i; j++)
                {
                    // Out of range
                    if (m_fieldLineSize < m_posY + i || m_fieldColumnSize < m_posX + j) return;

                    // Cannot be seen from Agent
                    if (fop <= j && j <= mop)
                    {
                        continue;
                    }

                    // Can be seen from Agent
                    else
                    {
                        skip = false;
                    }

                    // Empty
                    if (m_fieldData[m_posY + i][m_posX + j] == 1)
                    {
                        if (j != 0) observationList[m_sight + i][m_sight + j] = 1;
                    }

                    // Exit
                    else if (m_fieldData[m_posY + i][m_posX + j] == 2)
                    {
                        if (j != 0) observationList[m_sight + i][m_sight + j] = 2;
                        if (!skip)
                        {
                            if (fop == -1) fop = j;
                            else mop = j;
                        }
                    }

                    // Obstacle
                    else if (m_fieldData[m_posY + i][m_posX + j] == 3)
                    {
                        if (j != 0) observationList[m_sight + i][m_sight + j] = 3;
                        if (!skip)
                        {
                            if (fop == -1) fop = j;
                            else mop = j;
                        }
                    }

                    // Agent
                    else if (10 <= m_fieldData[m_posY + i][m_posX + j] && m_fieldData[m_posY + i][m_posX + j] < 10 + m_agentCnt)
                    {
                        if (j != 0) observationList[m_sight + i][m_sight + j] = 4;
                        if (!skip)
                        {
                            if (fop == -1) fop = j;
                            else mop = j;
                        }
                    }
                }
            }
        }

        // Back left
        else if (dir == 4)
        {
            // Neighboorhood is not empty
            if (observationNeighborhood[7] != 1) return;

            int fop = -1;  // First obstacle pos
            int mop = -1;  // Max obstacle pos
            bool skip = false;

            for (int i = 2; i <= m_sight; i++)
            {
                if (fop != -1 && mop == -1) mop = fop;

                if (mop != -1)
                {
                    mop++;
                    skip = true;
                }

                for (int j = 0; j < i; j++)
                {
                    // Out of range
                    if (m_fieldLineSize < m_posY + i || m_posX - j < 0) return;

                    // Cannot be seen from Agent
                    if (fop <= j && j <= mop)
                    {
                        continue;
                    }

                    // Can be seen from Agent
                    else
                    {
                        skip = false;
                    }

                    // Empty
                    if (m_fieldData[m_posY + i][m_posX - j] == 1)
                    {
                        if (j != 0) observationList[m_sight + i][m_sight - j] = 1;
                    }

                    // Exit
                    else if (m_fieldData[m_posY + i][m_posX - j] == 2)
                    {
                        if (j != 0) observationList[m_sight + i][m_sight - j] = 2;
                        if (!skip)
                        {
                            if (fop == -1) fop = j;
                            else mop = j;
                        }
                    }

                    // Obstacle
                    else if (m_fieldData[m_posY + i][m_posX - j] == 3)
                    {
                        if (j != 0) observationList[m_sight + i][m_sight - j] = 3;
                        if (!skip)
                        {
                            if (fop == -1) fop = j;
                            else mop = j;
                        }
                    }

                    // Agent
                    else if (10 <= m_fieldData[m_posY + i][m_posX - j] && m_fieldData[m_posY + i][m_posX - j] < 10 + m_agentCnt)
                    {
                        if (j != 0) observationList[m_sight + i][m_sight - j] = 4;
                        if (!skip)
                        {
                            if (fop == -1) fop = j;
                            else mop = j;
                        }
                    }
                }
            }
        }

        // Left back
        else if (dir == 5)
        {
            // Neighboorhood is not empty
            if (observationNeighborhood[3] != 1) return;

            int fop = -1;  // First obstacle pos
            int mop = -1;  // Max obstacle pos
            bool skip = false;

            for (int j = 2; j <= m_sight; j++)
            {
                if (fop != -1 && mop == -1) mop = fop;

                if (mop != -1)
                {
                    mop++;
                    skip = true;
                }

                for (int i = 0; i < j; i++)
                {
                    // Out of range
                    if (m_fieldLineSize < m_posY + i || m_posX - j < 0) return;

                    // Cannot be seen from Agent
                    if (fop <= i && i <= mop)
                    {
                        continue;
                    }

                    // Can be seen from Agent
                    else
                    {
                        skip = false;
                    }

                    // Empty
                    if (m_fieldData[m_posY + i][m_posX - j] == 1)
                    {
                        if (i != 0) observationList[m_sight + i][m_sight - j] = 1;
                    }

                    // Exit
                    else if (m_fieldData[m_posY + i][m_posX - j] == 2)
                    {
                        if (i == 0) observationList[m_sight + i][m_sight - j] = 2;
                        if (!skip)
                        {
                            if (fop == -1) fop = i;
                            else mop = i;
                        }
                    }

                    // Obstacle
                    else if (m_fieldData[m_posY + i][m_posX - j] == 3)
                    {
                        if (i != 0) observationList[m_sight + i][m_sight - j] = 3;
                        if (!skip)
                        {
                            if (fop == -1) fop = i;
                            else mop = i;
                        }
                    }

                    // Agent
                    else if (10 <= m_fieldData[m_posY + i][m_posX - j] && m_fieldData[m_posY + i][m_posX - j] < 10 + m_agentCnt)
                    {
                        if (i != 0) observationList[m_sight + i][m_sight - j] = 4;
                        if (!skip)
                        {
                            if (fop == -1) fop = i;
                            else mop = i;
                        }
                    }
                }
            }
        }

        // Left forward
        else if (dir == 6)
        {
            // Neighboorhood is not empty
            if (observationNeighborhood[3] != 1) return;

            int fop = -1;  // First obstacle pos
            int mop = -1;  // Max obstacle pos
            bool skip = false;

            for (int j = 2; j <= m_sight; j++)
            {
                if (fop != -1 && mop == -1) mop = fop;

                if (mop != -1)
                {
                    mop++;
                    skip = true;
                }

                for (int i = 0; i < j; i++)
                {
                    // Out of range
                    if (m_posY - i < 0 || m_posX - j < 0) return;

                    // Cannot be seen from Agent
                    if (fop <= i && i <= mop)
                    {
                        continue;
                    }

                    // Can be seen from Agent
                    else
                    {
                        skip = false;
                    }

                    // Empty
                    if (m_fieldData[m_posY - i][m_posX - j] == 1)
                    {
                        if (i != 0) observationList[m_sight - i][m_sight - j] = 1;
                    }

                    // Exit
                    else if (m_fieldData[m_posY - i][m_posX - j] == 2)
                    {
                        if (i != 0) observationList[m_sight - i][m_sight - j] = 2;
                        if (!skip)
                        {
                            if (fop == -1) fop = i;
                            else mop = i;
                        }
                    }

                    // Obstacle
                    else if (m_fieldData[m_posY - i][m_posX - j] == 3)
                    {
                        if (i != 0) observationList[m_sight - i][m_sight - j] = 3;
                        if (!skip)
                        {
                            if (fop == -1) fop = i;
                            else mop = i;
                        }
                    }

                    // Agent
                    else if (10 <= m_fieldData[m_posY - i][m_posX - j] && m_fieldData[m_posY - i][m_posX - j] < 10 + m_agentCnt)
                    {
                        if (i != 0) observationList[m_sight - i][m_sight - j] = 4;
                        if (!skip)
                        {
                            if (fop == -1) fop = i;
                            else mop = i;
                        }
                    }
                }
            }
        }

        // Forward left
        else if (dir == 7)
        {
            // Neighboorhood is not empty
            if (observationNeighborhood[1] != 1) return;

            int fop = -1;  // First obstacle pos
            int mop = -1;  // Max obstacle pos
            bool skip = false;

            for (int i = 2; i <= m_sight; i++)
            {
                if (fop != -1 && mop == -1) mop = fop;

                if (mop != -1)
                {
                    mop++;
                    skip = true;
                }

                for (int j = 0; j < i; j++)
                {
                    // Out of range
                    if (m_posY - i < 0 || m_posX - j < 0) return;

                    // Cannot be seen from Agent
                    if (fop <= j && j <= mop)
                    {
                        continue;
                    }

                    // Can be seen from Agent
                    else
                    {
                        skip = false;
                    }

                    // Empty
                    if (m_fieldData[m_posY - i][m_posX - j] == 1)
                    {
                        if (j != 0) observationList[m_sight - i][m_sight - j] = 1;
                    }

                    // Exit
                    else if (m_fieldData[m_posY - i][m_posX - j] == 2)
                    {
                        if (j != 0) observationList[m_sight - i][m_sight - j] = 2;
                        if (!skip)
                        {
                            if (fop == -1) fop = j;
                            else mop = j;
                        }
                    }

                    // Obstacle
                    else if (m_fieldData[m_posY - i][m_posX - j] == 3)
                    {
                        if (j != 0) observationList[m_sight - i][m_sight - j] = 3;
                        if (!skip)
                        {
                            if (fop == -1) fop = j;
                            else mop = j;
                        }
                    }

                    // Agent
                    else if (10 <= m_fieldData[m_posY - i][m_posX - j] && m_fieldData[m_posY - i][m_posX - j] < 10 + m_agentCnt)
                    {
                        if (j != 0) observationList[m_sight - i][m_sight - j] = 4;
                        if (!skip)
                        {
                            if (fop == -1) fop = j;
                            else mop = j;
                        }
                    }
                }
            }
        }
    }
}
