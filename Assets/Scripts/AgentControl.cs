using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [HideInInspector] public int agent_tilemapData_x;
    [HideInInspector] public int agent_tilemapData_y;

    private ObservationAroundAgent observation;
    private bool[] neighborhoodInfo;


    // Initializing.
    public override void Initialize()
    {
        Transform TrainingArea = transform.parent;
        settings = TrainingArea.GetComponentInChildren<Settings>();
        fieldControl = TrainingArea.GetComponentInChildren<FieldControl>();
    }


    // Set agentPos.
    public override void OnEpisodeBegin()
    {
        int n = (int)this.gameObject.name[5] - 48;
        agent_id = fieldControl.agentsData[n].m_id;

        for (int i = 0; i < settings.fieldHeight; i++)
        {
            for (int j = 0; j < settings.fieldWidth; j++)
            {
                if (fieldControl.fieldData[i][j] == agent_id)
                {
                    agent_tilemapData_x = j;
                    agent_tilemapData_y = i;
                }
            }
        }

        observation = new(
            fieldControl.fieldData,
            settings.fieldHeight,
            settings.fieldWidth,
            agent_tilemapData_x,
            agent_tilemapData_y,
            settings.agentSight,
            settings.agentCnt
        );
    }


    // Agent collect observation.
    public override void CollectObservations(VectorSensor sensor)
    {
        if (fieldControl.agentsData[agent_id - 10].m_active)
        {
            observation.UpdateObservation(agent_tilemapData_x, agent_tilemapData_y);

            // For debug
            if (settings.debugMode) observation.PrintAgentObservation(agent_id);

            for (int i = 0; i < settings.agentSight * 2 + 1; i++)
            {
                for (int j = 0; j < settings.agentSight * 2 + 1; j++)
                {
                    sensor.AddObservation(observation.observationList[i][j]);
                }
            }
        }
    }

    
    // Agent moves.
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (fieldControl.agentsData[agent_id - 10].m_active)
        {
            CheckAgentReachGoal(agent_id);

            var action = actions.DiscreteActions;

            // Forward
            if (action[0] == 1)
            {
                fieldControl.MoveAgentTile(agent_id, 1);
                agent_tilemapData_y--;
            }

            // Right
            else if (action[0] == 2)
            {
                fieldControl.MoveAgentTile(agent_id, 2);
                agent_tilemapData_x++;
            }

            // Back
            else if (action[0] == 3)
            {
                fieldControl.MoveAgentTile(agent_id, 3);
                agent_tilemapData_y++;
            }

            // Left
            else if (action[0] == 4)
            {
                fieldControl.MoveAgentTile(agent_id, 4);
                agent_tilemapData_x--;
            }
        }
    }

    
    /// <summary>
    /// Get information of Agent's neighborhood.
    /// </summary>
    private void GetNeighborhoodInfo()
    {
        neighborhoodInfo = Enumerable.Repeat<bool>(true, 4).ToArray();

        if (observation.observationList[settings.agentSight - 1][settings.agentSight] != 1
            && observation.observationList[settings.agentSight - 1][settings.agentSight] != 2) neighborhoodInfo[0] = false;

        if (observation.observationList[settings.agentSight][settings.agentSight + 1] != 1
            && observation.observationList[settings.agentSight][settings.agentSight + 1] != 2) neighborhoodInfo[1] = false;

        if (observation.observationList[settings.agentSight + 1][settings.agentSight] != 1
            && observation.observationList[settings.agentSight + 1][settings.agentSight] != 2) neighborhoodInfo[2] = false;

        if (observation.observationList[settings.agentSight][settings.agentSight - 1] != 1
            && observation.observationList[settings.agentSight][settings.agentSight - 1] != 2) neighborhoodInfo[3] = false;
    }


    // Agent cannot move to where tile is not empty.
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        GetNeighborhoodInfo();

        if (!neighborhoodInfo[0]) actionMask.SetActionEnabled(0, 0, false);
        if (!neighborhoodInfo[1]) actionMask.SetActionEnabled(0, 1, false);
        if (!neighborhoodInfo[2]) actionMask.SetActionEnabled(0, 2, false);
        if (!neighborhoodInfo[3]) actionMask.SetActionEnabled(0, 3, false);
    }


    /// <summary>
    /// Check whether Agent reached the goal or not.
    /// </summary>
    /// <param name="agent_id">Agent's id</param>
    public void CheckAgentReachGoal(int agent_id)
    {

        int i = fieldControl.agentsData[agent_id].m_position.m_tilemapData_x;
        int j = fieldControl.agentsData[agent_id].m_position.m_tilemapData_y;

        if (fieldControl.fieldData[i][j] == 2)
        {
            AddReward(1.0f);

            fieldControl.agentsData[agent_id - 10].m_active = false;

            fieldControl.activeAgentsNum--;

            if (fieldControl.activeAgentsNum == 0)
            {
                EndEpisode();

                fieldControl.InitializeField();
            }
        }

        AddReward(-1.0f / 5000 / settings.agentCnt);
    }


    // Human Control
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;

        GetNeighborhoodInfo();

        if (neighborhoodInfo[0] && Input.GetKey(KeyCode.W)) discreteActionsOut[0] = 1;
        if (neighborhoodInfo[1] && Input.GetKey(KeyCode.D)) discreteActionsOut[0] = 2;
        if (neighborhoodInfo[2] && Input.GetKey(KeyCode.S)) discreteActionsOut[0] = 3;
        if (neighborhoodInfo[3] && Input.GetKey(KeyCode.A)) discreteActionsOut[0] = 4;
    }
}
