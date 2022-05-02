using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FieldControl : MonoBehaviour
{
    Settings settings;

    public GameObject TrainingArea;

    List<List<int>> fieldData = new List<List<int>>();
    public List<Sprite> tilemapSprites;
    public Tilemap field_tilemap;

    public Tilemap agent_tilemap;
    public TileBase agent_tile;
    public GameObject agent;


    private void Start()
    {
        settings = GameObject.Find("Settings").GetComponent<Settings>();

        ResetFieldData(settings.fieldWidth, settings.fieldHeight);
        RandomSetAgent(settings.fieldWidth, settings.fieldHeight, settings.agentCnt);

        // For debug
        PrintFieldData(settings.fieldWidth, settings.fieldHeight);
    }

    private void Update()
    {

    }


    /// <summary>
    /// Initializing Field data.
    /// </summary>
    /// <param name="width">Field's width</param>
    /// <param name="height">Field's height</param>
    public void ResetFieldData(int width, int height)
    {
        for (int i = 0; i < height; i++)
        {
            List<int> temp = new List<int>();
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
                Vector3Int pos = new Vector3Int(-width / 2 + j, height / 2 - 1 - i, 0);

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
        int cnt = 0;
        while (cnt < num)
        {
            Vector3Int pos = new Vector3Int(Random.Range(-width / 2, width / 2), Random.Range(height / 2 - 1, -height / 2 + 1), 0);
            
            // Only Empty position
            if (field_tilemap.GetSprite(pos) == tilemapSprites[1])
            {
                agent_tilemap.SetTile(pos, agent_tile);
                fieldData[height / 2 - 1 - pos.y][pos.x + width / 2] = 10 + cnt;

                GameObject obj = (GameObject)Instantiate(agent);
                obj.transform.parent = TrainingArea.transform;
                AgentControl agentControl = obj.GetComponent<AgentControl>();
                agentControl.agent_id = 10 + cnt;

                cnt++;
            }
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
