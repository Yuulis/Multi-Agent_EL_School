using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentControl : Agent
{
    // Settings
    Settings settings;

    // FieldControl
    FieldControl fieldControl;

    // Agent's data
    [HideInInspector] public int agent_id;
    [HideInInspector] public int agent_tilemapIndex_x;
    [HideInInspector] public int agent_tilemapIndex_y;

    // Agent's observation
    private ObservationAroundAgent observation;
    private readonly List<bool> observationNeighborhood;


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

        for (int y = 0; y < settings.fieldHeight; y++)
        {
            for (int x = 0; x < settings.fieldWidth; x++)
            {
                if (fieldControl.fieldData2D[y][x].tileNum == agent_id)
                {
                    agent_tilemapIndex_x = x;
                    agent_tilemapIndex_y = y;
                }
            }
        }

        observation = new(
            fieldControl.fieldData2D,
            settings.fieldHeight,
            settings.fieldWidth,
            agent_tilemapIndex_x,
            agent_tilemapIndex_y,
            settings.agentSight,
            settings.agentCnt
        );

    }


    // Agent collect observation.
    public override void CollectObservations(VectorSensor sensor)
    {
        if (fieldControl.agentsData[agent_id - 10].m_active)
        {
            observation.UpdateObservation(agent_tilemapIndex_x, agent_tilemapIndex_y);

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
                agent_tilemapIndex_y--;
            }

            // Right
            else if (action[0] == 2)
            {
                fieldControl.MoveAgentTile(agent_id, 2);
                agent_tilemapIndex_x++;
            }

            // Back
            else if (action[0] == 3)
            {
                fieldControl.MoveAgentTile(agent_id, 3);
                agent_tilemapIndex_y++;
            }

            // Left
            else if (action[0] == 4)
            {
                fieldControl.MoveAgentTile(agent_id, 4);
                agent_tilemapIndex_x--;
            }
        }
    }

    
    /// <summary>
    /// Get information of Agent's neighborhood.
    /// </summary>
    private void GetNeighborhoodInfo()
    {
        if (observation.observationList[settings.agentSight - 1][settings.agentSight] != 1
            && observation.observationList[settings.agentSight - 1][settings.agentSight] != 2) observationNeighborhood[0] = false;

        if (observation.observationList[settings.agentSight][settings.agentSight + 1] != 1
            && observation.observationList[settings.agentSight][settings.agentSight + 1] != 2) observationNeighborhood[1] = false;

        if (observation.observationList[settings.agentSight + 1][settings.agentSight] != 1
            && observation.observationList[settings.agentSight + 1][settings.agentSight] != 2) observationNeighborhood[2] = false;

        if (observation.observationList[settings.agentSight][settings.agentSight - 1] != 1
            && observation.observationList[settings.agentSight][settings.agentSight - 1] != 2) observationNeighborhood[3] = false;
    }


    // Agent cannot move to where tile is not empty.
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        GetNeighborhoodInfo();

        if (!observationNeighborhood[1]) actionMask.SetActionEnabled(0, 0, false);
        if (!observationNeighborhood[3]) actionMask.SetActionEnabled(0, 1, false);
        if (!observationNeighborhood[5]) actionMask.SetActionEnabled(0, 2, false);
        if (!observationNeighborhood[8]) actionMask.SetActionEnabled(0, 3, false);
    }


    /// <summary>
    /// Check whether Agent reached the goal or not.
    /// </summary>
    /// <param name="agent_id">Agent's id</param>
    public void CheckAgentReachGoal(int agent_id)
    {

        int i = fieldControl.agentsData[agent_id].m_position.m_fieldDataIndex_y;
        int j = fieldControl.agentsData[agent_id].m_position.m_fieldDataIndex_x;

        if (fieldControl.fieldData2D[i][j].tileNum == 2)
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

        if (observationNeighborhood[0] && Input.GetKey(KeyCode.W)) discreteActionsOut[0] = 1;
        if (observationNeighborhood[1] && Input.GetKey(KeyCode.D)) discreteActionsOut[0] = 2;
        if (observationNeighborhood[2] && Input.GetKey(KeyCode.S)) discreteActionsOut[0] = 3;
        if (observationNeighborhood[3] && Input.GetKey(KeyCode.A)) discreteActionsOut[0] = 4;
    }
}
