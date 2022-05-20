using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
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
        ObservationAroundAgent observation = new ObservationAroundAgent(
            fieldControl.fieldData,
            settings.fieldHeight,
            settings.fieldWidth, 
            agentPos_x,
            agentPos_y,
            settings.agentSight,
            settings.agentCnt
        );

        observation.PrintAgentObservation(agent_id);

        for (int i = 0; i < settings.agentSight * 2 + 1; i++)
        {
            for (int j = 0; j < settings.agentSight * 2 + 1; j++)
            {
                sensor.AddObservation(observation.m_observationList[i][j]);
            }
        }
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
