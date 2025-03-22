using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PatrolRigidbody", story: "[Agent] patrol along [waypoints] using its rigidbody", category: "Action/Navigation", id: "1c819a937c3a96d400dd47bc526e8a5e")]
public partial class PatrolRigidbodyAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<List<GameObject>> Waypoints;
    [SerializeReference] public BlackboardVariable<float> Speed;
    [SerializeReference] public BlackboardVariable<float> WaypointWaitTime = new BlackboardVariable<float>(1.0f);
    [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new BlackboardVariable<float>(0.2f);
    [Tooltip("Should patrol restart from the latest point?")]
    [SerializeReference] public BlackboardVariable<bool> PreserveLatestPatrolPoint = new(false);

    private Rigidbody m_Rigidbody;

    [CreateProperty] private Vector3 m_CurrentTarget;
    [CreateProperty] private int m_CurrentPatrolPoint = 0;
    [CreateProperty] private bool m_Waiting;
    [CreateProperty] private float m_WaypointWaitTimer;

    protected override Status OnStart()
    {
        if (Agent.Value == null)
        {
            LogFailure("No agent assigned.");
            return Status.Failure;
        }

        if (Waypoints.Value == null || Waypoints.Value.Count == 0)
        {
            LogFailure("No waypoints to patrol assigned.");
            return Status.Failure;
        }

        Initialize();

        m_Waiting = false;
        m_WaypointWaitTimer = 0.0f;

        MoveToNextWaypoint();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Agent.Value == null || Waypoints.Value == null)
        {
            return Status.Failure;
        }

        if (m_Waiting)
        {
            if (m_WaypointWaitTimer > 0.0f)
            {
                m_WaypointWaitTimer -= Time.deltaTime;
            }
            else
            {
                m_WaypointWaitTimer = 0f;
                m_Waiting = false;
                MoveToNextWaypoint();
            }
        }
        else
        {
            float distance = GetDistanceToWaypoint();
            Vector3 agentPosition = Agent.Value.transform.position;

            if (distance <= DistanceThreshold)
            {
                m_WaypointWaitTimer = WaypointWaitTime.Value;
                m_Waiting = true;
            }
            else
            {
                MoveAgent(Speed.Value);
            }
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {

    }

    protected override void OnDeserialize()
    {
        Initialize();
    }

    private void Initialize()
    {
        m_Rigidbody = Agent.Value.GetComponent<Rigidbody>();
        if (m_Rigidbody == null)
        {
            LogFailure("Agent is missing a Rigidbody component.");
        }

        m_CurrentPatrolPoint = PreserveLatestPatrolPoint.Value ? m_CurrentPatrolPoint - 1 : -1;
    }

    private float GetDistanceToWaypoint()
    {
        Vector3 agentPosition = Agent.Value.transform.position;
        Vector3 targetPosition = m_CurrentTarget;

        agentPosition.y = targetPosition.y; // Ignore Y-axis for distance check.
        return Vector3.Distance(agentPosition, targetPosition);
    }

    private void MoveAgent(float speed)
    {
        if (m_Rigidbody == null) return;

        Vector3 agentPosition = Agent.Value.transform.position;
        Vector3 toDestination = m_CurrentTarget - agentPosition;
        toDestination.y = 0.0f;
        toDestination.Normalize();

        Vector3 newPosition = agentPosition + toDestination * (speed * Time.deltaTime);
        m_Rigidbody.MovePosition(newPosition);

        // Rotate towards the movement direction
        if (toDestination.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(toDestination);
            m_Rigidbody.MoveRotation(targetRotation);
        }
    }

    private void MoveToNextWaypoint()
    {
        m_CurrentPatrolPoint = (m_CurrentPatrolPoint + 1) % Waypoints.Value.Count;
        m_CurrentTarget = Waypoints.Value[m_CurrentPatrolPoint].transform.position;
    }
}

