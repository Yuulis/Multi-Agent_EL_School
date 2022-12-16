using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentControlByPoca : Agent
{
    // Settings
    Settings settings;

    // FieldControl
    FieldControlByPoca fieldControl;

    // Agent's data
    private int agent_id;

    // Agent's floor
    private int floorNum;

    // Agent's observation
    private ObservationAroundAgent observation;


    // Initializing
    public override void Initialize()
    {
        Transform TrainingArea = transform.parent;
        settings = TrainingArea.GetComponentInChildren<Settings>();
        fieldControl = TrainingArea.GetComponentInChildren<FieldControlByPoca>();
    }


    // When an episode begins
    public override void OnEpisodeBegin()
    {
        int n = (int)this.gameObject.name[5] - 48;
        agent_id = fieldControl.agentsInfo[n].m_id;
        floorNum = fieldControl.agentsInfo[agent_id - 1000].m_floorNum;

        Vector2Int positionIndex = fieldControl.agentsInfo[agent_id - 1000].m_positionIndex;
        observation = new(
            fieldControl.fieldDataList,
            fieldControl.fieldAgentData,
            settings.fieldHeight,
            settings.fieldWidth,
            positionIndex,
            settings.agentSight
        );
    }


    // collect observations
    public override void CollectObservations(VectorSensor sensor)
    {
        observation.UpdateObservation(fieldControl.agentsInfo[agent_id - 1000].m_positionIndex, settings.agentSight);

        // For debug
        if (settings.debugMode) observation.PrintAgentObservation(agent_id);

        for (int i = 0; i < settings.agentSight * 2 + 1; i++)
        {
            for (int j = 0; j < settings.agentSight * 2 + 1; j++)
            {
                if (fieldControl.agentsInfo[agent_id - 1000].m_active)
                {
                    sensor.AddObservation(observation.observationList[i][j]);
                }
                else
                {
                    sensor.AddObservation(-1);
                }
            }
        }
    }


    // When agent moves
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (fieldControl.agentsInfo[agent_id - 1000].m_active)
        {
            CheckAgentReachGoal(agent_id);

            var action = actions.DiscreteActions;

            // Forward
            if (action[0] == 1)
            {
                fieldControl.MoveAgentTile(floorNum, agent_id, 1);
            }

            // Back
            else if (action[0] == 2)
            {
                fieldControl.MoveAgentTile(floorNum, agent_id, 2);
            }

            // Right
            else if (action[0] == 3)
            {
                fieldControl.MoveAgentTile(floorNum, agent_id, 3);
            }

            // Left
            else if (action[0] == 4)
            {
                fieldControl.MoveAgentTile(floorNum, agent_id, 4);
            }
        }
        else
        {
            Vector2Int positionIndex = fieldControl.agentsInfo[agent_id - 1000].m_positionIndex;
            Vector3Int pos = new(positionIndex.x, settings.fieldHeight - positionIndex.y, 0);
            fieldControl.agentTilemapList[floorNum].SetTile(pos, null);
            fieldControl.fieldAgentDataList[floorNum][positionIndex.y][positionIndex.x] = false;
            fieldControl.agentsInfo[agent_id - 1000].m_active = false;
        }
    }


    // Agent cannot move to where tile is not empty or exit.
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        if (!observation.observationListNeighborhood[1]) actionMask.SetActionEnabled(0, 1, false);
        if (!observation.observationListNeighborhood[7]) actionMask.SetActionEnabled(0, 2, false);
        if (!observation.observationListNeighborhood[5]) actionMask.SetActionEnabled(0, 3, false);
        if (!observation.observationListNeighborhood[3]) actionMask.SetActionEnabled(0, 4, false);
    }


    /// <summary>
    /// Check whether Agent reached the goal or not.
    /// </summary>
    /// <param name="agent_id">Agent's id</param>
    public void CheckAgentReachGoal(int agent_id)
    {
        Vector2Int positionIndex = fieldControl.agentsInfo[agent_id - 1000].m_positionIndex;

        if (fieldControl.fieldDataList[0][positionIndex.y][positionIndex.x] == 2)
        {
            fieldControl.ReachedExit();

            Vector3Int pos = new(positionIndex.x, settings.fieldHeight - positionIndex.y, 0);

            fieldControl.agentTilemapList[floorNum].SetTile(pos, null);
            fieldControl.fieldAgentDataList[floorNum][positionIndex.y][positionIndex.x] = false;
            fieldControl.agentsInfo[agent_id - 1000].m_active = false;
            fieldControl.activeAgentsNum--;

            // When all agents reached any exit
            if (fieldControl.activeAgentsNum == 0)
            {
                fieldControl.AllReachedExit();
                fieldControl.InitializeTileMaps(settings.fieldHeight, settings.fieldWidth);
            }
        }
    }


    // Human Control
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;

        if (observation.observationListNeighborhood[1] && Input.GetKey(KeyCode.W)) discreteActionsOut[0] = 1;
        if (observation.observationListNeighborhood[7] && Input.GetKey(KeyCode.S)) discreteActionsOut[0] = 2;
        if (observation.observationListNeighborhood[5] && Input.GetKey(KeyCode.D)) discreteActionsOut[0] = 3;
        if (observation.observationListNeighborhood[3] && Input.GetKey(KeyCode.A)) discreteActionsOut[0] = 4;
    }
}
