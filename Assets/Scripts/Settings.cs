using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public int fieldWidth = 30;
    public int fieldHeight = 20;

    public int agentCnt = 10;

    // This parameter is depended on vector observation space size of agent.
    // Space size = (agentSight * 2 + 1) ^ 2 .
    public int agentSight = 5;

    public bool debugMode = false;
}
