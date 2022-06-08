using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public int fieldWidth = 30;
    public int fieldHeight = 20;

    // Set this value of Agent prefabs on the scene before start.
    public int agentCnt = 10;

    // This parameter is depended on vector observation space size of agent.
    // Space size = (agentSight * 2 + 1) ^ 2 .
    public int agentSight = 5;

    // For debug.
    public bool debugMode = false;
}
