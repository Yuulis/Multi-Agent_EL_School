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
    private int agent_id;
    private Vector2Int positionIndex;

    // Agent's observation
    private ObservationAroundAgent observation;
    private readonly List<bool> observationNeighborhood;


    // Initializing
    public override void Initialize()
    {
        Transform TrainingArea = transform.parent;
        settings = TrainingArea.GetComponentInChildren<Settings>();
        fieldControl = TrainingArea.GetComponentInChildren<FieldControl>();
    }


    // When an episode begins
    public override void OnEpisodeBegin()
    {
        int n = (int)this.gameObject.name[5] - 48;
        agent_id = fieldControl.agentsInfo[n].m_id;
        positionIndex = fieldControl.agentsInfo[agent_id - 10].m_positionIndex;

        observation = new(
            fieldControl.fieldData2D,
            settings.fieldHeight,
            settings.fieldWidth,
            positionIndex,
            settings.agentSight,
            settings.agentCnt
        );
    }


    // collect observations
    public override void CollectObservations(VectorSensor sensor)
    {
        if (fieldControl.agentsInfo[agent_id - 10].m_active)
        {
            observation.UpdateObservation(positionIndex, settings.agentSight);

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

    
    // When agent moves
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (fieldControl.agentsInfo[agent_id - 10].m_active)
        {
            CheckAgentReachGoal(agent_id);

            var action = actions.DiscreteActions;

            // Forward
            if (action[0] == 1)
            {
                fieldControl.MoveAgentTile(agent_id, 1);
                positionIndex.y--;
            }

            // Right
            else if (action[0] == 2)
            {
                fieldControl.MoveAgentTile(agent_id, 2);
                positionIndex.x++;
            }

            // Back
            else if (action[0] == 3)
            {
                fieldControl.MoveAgentTile(agent_id, 3);
                positionIndex.y++;
            }

            // Left
            else if (action[0] == 4)
            {
                fieldControl.MoveAgentTile(agent_id, 4);
                positionIndex.x--;
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


    // Agent cannot move to where tile is not empty or exit.
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        // GetNeighborhoodInfo();

        if (!observation.observationListNeighborhood[1]) actionMask.SetActionEnabled(0, 0, false);
        if (!observation.observationListNeighborhood[3]) actionMask.SetActionEnabled(0, 1, false);
        if (!observation.observationListNeighborhood[5]) actionMask.SetActionEnabled(0, 2, false);
        if (!observation.observationListNeighborhood[7]) actionMask.SetActionEnabled(0, 3, false);
    }


    /// <summary>
    /// Check whether Agent reached the goal or not.
    /// </summary>
    /// <param name="agent_id">Agent's id</param>
    public void CheckAgentReachGoal(int agent_id)
    {
        if (fieldControl.fieldData2D[positionIndex.y][positionIndex.x] == 2)
        {
            AddReward(1.0f);

            fieldControl.agentsInfo[agent_id - 10].m_active = false;

            fieldControl.activeAgentsNum--;

            if (fieldControl.activeAgentsNum == 0)
            {
                EndEpisode();

                fieldControl.InitializeTileMaps();
            }
        }

        AddReward(-1.0f / 5000 / settings.agentCnt);
    }


    // Human Control
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;

        // GetNeighborhoodInfo();

        if (observation.observationListNeighborhood[1] && Input.GetKey(KeyCode.W)) discreteActionsOut[0] = 1;
        if (observation.observationListNeighborhood[3] && Input.GetKey(KeyCode.D)) discreteActionsOut[0] = 2;
        if (observation.observationListNeighborhood[5] && Input.GetKey(KeyCode.S)) discreteActionsOut[0] = 3;
        if (observation.observationListNeighborhood[7] && Input.GetKey(KeyCode.A)) discreteActionsOut[0] = 4;
    }
}
