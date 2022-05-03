using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentControl : Agent
{
    Settings settings;

    FieldControl fieldControl;

    [HideInInspector] public int agent_id;
    private int agentPos_x = 0;
    private int agentPos_y = 0;

    // Called when this agent was instantiated 
    private void Start()
    {
        Transform TrainingArea = transform.parent;
        settings = TrainingArea.GetComponentInChildren<Settings>();
        fieldControl = TrainingArea.GetComponentInChildren<FieldControl>();
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void OnEpisodeBegin()
    {
        for (int i = 0; i < settings.fieldHeight; i++)
        {
            for (int j = 0; j < settings.fieldWidth; j++)
            {
                if (fieldControl.fieldData[i][j] == agent_id)
                {
                    agentPos_x = j;
                    agentPos_y = i;
                }
            }
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        
    }

    public List<List<int>> GetFieldDataAroundAgent(int agentPos_x, int agentPos_y, int width, int height)
    {
        List<List<int>> list = new List<List<int>>();
        for (int i = 0; i < height; i++)
        {
            List<int> temp = new List<int>();
            for (int j = 0; j < width; j++)
            {
                temp.Add(0);
            }
            list.Add(temp);
        }
        list[height / 2][width / 2] = 1;

        GetFieldDataAroundAgentType1(agentPos_x, agentPos_y, 0, list);

        return list;
    }

    public void GetFieldDataAroundAgentType1(int agentPos_x, int agentPos_y, int dir, List<List<int>> list)
    {

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        base.Heuristic(actionsOut);
    }
}
