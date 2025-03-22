using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "NavigateToTargetRigidbody", story: "[Agent] navigates to [target] using its rigidbody", category: "Action/Navigation", id: "041759194b6fd946673c092b282e5b34")]
public partial class NavigateToTargetRigidbodyAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(1.0f);
    [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new BlackboardVariable<float>(0.2f);

    // This will only be used for gradual stopping behavior.
    [SerializeReference] public BlackboardVariable<float> SlowDownDistance = new BlackboardVariable<float>(1.0f);

    private Rigidbody m_Rigidbody;
    private Vector3 m_LastTargetPosition;
    private Vector3 m_ColliderAdjustedTargetPosition;
    private float m_ColliderOffset;

    protected override Status OnStart()
    {
        if (Agent.Value == null || Target.Value == null)
        {
            return Status.Failure;
        }

        return Initialize();
    }

    protected override Status OnUpdate()
    {
        if (Agent.Value == null || Target.Value == null)
        {
            return Status.Failure;
        }

        // Check if the target position has changed
        bool updateTargetPosition = !Mathf.Approximately(m_LastTargetPosition.x, Target.Value.transform.position.x) ||
                                    !Mathf.Approximately(m_LastTargetPosition.y, Target.Value.transform.position.y) ||
                                    !Mathf.Approximately(m_LastTargetPosition.z, Target.Value.transform.position.z);

        if (updateTargetPosition)
        {
            m_LastTargetPosition = Target.Value.transform.position;
            m_ColliderAdjustedTargetPosition = GetPositionColliderAdjusted();
        }

        float distance = GetDistanceXZ();
        if (distance <= (DistanceThreshold + m_ColliderOffset))
        {
            return Status.Success;
        }

        float speed = Speed;

        if (SlowDownDistance > 0.0f && distance < SlowDownDistance)
        {
            float ratio = distance / SlowDownDistance;
            speed = Mathf.Max(0.1f, Speed * ratio);
        }

        MoveAgent(speed);

        return Status.Running;
    }

    protected override void OnEnd()
    {
        
    }

    protected override void OnDeserialize()
    {
        Initialize();
    }

    private Status Initialize()
    {
        m_LastTargetPosition = Target.Value.transform.position;
        m_ColliderAdjustedTargetPosition = GetPositionColliderAdjusted();

        // Compute the collider offset
        m_ColliderOffset = 0.0f;
        Collider agentCollider = Agent.Value.GetComponentInChildren<Collider>();
        if (agentCollider != null)
        {
            Vector3 colliderExtents = agentCollider.bounds.extents;
            m_ColliderOffset += Mathf.Max(colliderExtents.x, colliderExtents.z);
        }

        if (GetDistanceXZ() <= (DistanceThreshold + m_ColliderOffset))
        {
            return Status.Success;
        }

        // Get Rigidbody
        m_Rigidbody = Agent.Value.GetComponent<Rigidbody>();
        if (m_Rigidbody == null)
        {
            return Status.Failure;
        }

        return Status.Running;
    }

    private void MoveAgent(float speed)
    {
        if (m_Rigidbody == null) return;

        Vector3 agentPosition = Agent.Value.transform.position;
        Vector3 toDestination = m_ColliderAdjustedTargetPosition - agentPosition;
        toDestination.y = 0.0f;
        toDestination.Normalize();

        Vector3 newPosition = agentPosition + toDestination * (speed * Time.deltaTime);
        m_Rigidbody.MovePosition(newPosition);

        // Rotate towards the target
        if (toDestination.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(toDestination);
            m_Rigidbody.MoveRotation(targetRotation);
        }
    }

    private Vector3 GetPositionColliderAdjusted()
    {
        Collider targetCollider = Target.Value.GetComponentInChildren<Collider>();
        if (targetCollider != null)
        {
            return targetCollider.ClosestPoint(Agent.Value.transform.position);
        }
        return Target.Value.transform.position;
    }

    private float GetDistanceXZ()
    {
        Vector3 agentPosition = new Vector3(Agent.Value.transform.position.x, m_ColliderAdjustedTargetPosition.y, Agent.Value.transform.position.z);
        return Vector3.Distance(agentPosition, m_ColliderAdjustedTargetPosition);
    }
}

