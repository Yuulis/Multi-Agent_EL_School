using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    // Field size
    public int fieldWidth = 30;
    public int fieldHeight = 20;

    // Set this value of Agent prefabs on the scene before start.
    public int agentCnt = 20;

    // This parameter is depended on vector observation space size of agent.
    // Space size = (agentSight * 2 + 1) ^ 2 .
    public int agentSight = 5;

    // The list of the name of reading csv file.
    public List<string> csvFileName;

    // For debug.
    public bool debugMode = false;

    // Timescale
    public int timeScale = 1;

    // For dataCounter.
    public bool dataCountMode = false;
    public int maxEpisodeCount;
}
