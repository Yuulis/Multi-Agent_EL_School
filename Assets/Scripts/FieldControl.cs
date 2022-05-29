using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FieldControl : MonoBehaviour
{
    public GameObject Settings_obj;
    Settings settings;

    public GameObject TrainingArea;

    [HideInInspector] public List<List<int>> fieldData;
    public List<Sprite> tilemapSprites;

    public Tilemap field_tilemap;

    public Tilemap agent_tilemap;
    public TileBase agent_tile;

    public GameObject agent;
    [HideInInspector] public int activeAgentsNum;
    [HideInInspector] public List<Vector3Int> agentsPos;


    private void Awake()
    {
        settings = Settings_obj.GetComponent<Settings>();

        activeAgentsNum = settings.agentCnt;

        ResetFieldData(settings.fieldWidth, settings.fieldHeight);
        RandomSetAgent(settings.fieldWidth, settings.fieldHeight, settings.agentCnt);

        // For debug
        if (settings.debugMode) PrintFieldData(settings.fieldWidth, settings.fieldHeight);
    }


    /// <summary>
    /// Initializing Field data.
    /// </summary>
    /// <param name="width">Field's width</param>
    /// <param name="height">Field's height</param>
    public void ResetFieldData(int width, int height)
    {
        fieldData = new();

        for (int i = 0; i < height; i++)
        {
            List<int> temp = new();
            for (int j = 0; j < width; j++)
            {
                temp.Add(0);
            }
            fieldData.Add(temp);
        }

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector3Int pos = new(-width / 2 + j, height / 2 - 1 - i, 0);

                // Empty
                if (field_tilemap.GetSprite(pos) == tilemapSprites[1])
                {
                    fieldData[i][j] = 1;
                }

                // Exit
                else if (field_tilemap.GetSprite(pos) == tilemapSprites[2])
                {
                    fieldData[i][j] = 2;
                }

                // Obstacle
                else if (field_tilemap.GetSprite(pos) == tilemapSprites[3])
                {
                    fieldData[i][j] = 3;
                }
            }
        }
    }


    /// <summary>
    /// Agents set random place in field.
    /// </summary>
    /// <param name="width">Field's width</param>
    /// <param name="height">Field's height</param>
    /// <param name="num">Number of Agents</param>
    public void RandomSetAgent(int width, int height, int num)
    {
        agentsPos = new();

        int cnt = 0;
        while (cnt < num)
        {
            Vector3Int pos = new(Random.Range(-width / 2, width / 2), Random.Range(height / 2 - 1, -height / 2 + 1), 0);
            
            // Only Empty position
            if (field_tilemap.GetSprite(pos) == tilemapSprites[1])
            {
                agent_tilemap.SetTile(pos, agent_tile);
                fieldData[height / 2 - 1 - pos.y][pos.x + width / 2] = 10 + cnt;

                GameObject obj = (GameObject)Instantiate(agent);
                obj.transform.parent = TrainingArea.transform;
                AgentControl agentControl = obj.GetComponent<AgentControl>();

                agentControl.agent_id = 10 + cnt;
                agentsPos.Add(pos);

                cnt++;
            }
        }
    }


    /// <summary>
    /// Move agent_tile in the direction specified by dir.
    /// </summary>
    /// <param name="agent_id">Agent's id</param>
    /// <param name="dir">Direction of moving</param>
    public void MoveAgentTile(int agent_id, int dir)
    {
        Vector3Int pos = agentsPos[agent_id - 10];

        // Forward
        if (dir == 0)
        {
            Vector3Int newPos = new(pos.x, pos.y + 1, pos.z);
            agent_tilemap.SetTile(newPos, agent_tile);
            agent_tilemap.SetTile(pos, null);
        }

        // Right
        else if (dir == 1)
        {
            Vector3Int newPos = new(pos.x + 1, pos.y, pos.z);
            agent_tilemap.SetTile(newPos, agent_tile);
            agent_tilemap.SetTile(pos, null);
        }

        // Back
        else if (dir == 2)
        {
            Vector3Int newPos = new(pos.x, pos.y - 1, pos.z);
            agent_tilemap.SetTile(newPos, agent_tile);
            agent_tilemap.SetTile(pos, null);
        }

        // Left
        else if (dir == 3)
        {
            Vector3Int newPos = new(pos.x - 1, pos.y, pos.z);
            agent_tilemap.SetTile(newPos, agent_tile);
            agent_tilemap.SetTile(pos, null);
        }
    }


    /// <summary>
    /// Output Field data(for debug).
    /// </summary>
    /// <param name="width">Field's width</param>
    /// <param name="height">Field's height</param>
    public void PrintFieldData(int width, int height)
    {
        Debug.Log("===== Field data =====");
        for (int i = 0; i < height; i++)
        {
            string s = string.Empty;
            for (int j = 0; j < width; j++)
            {
                s += fieldData[i][j].ToString() + " ";
            }
            Debug.Log(s);
        }
        Debug.Log("======================");
    }
}
