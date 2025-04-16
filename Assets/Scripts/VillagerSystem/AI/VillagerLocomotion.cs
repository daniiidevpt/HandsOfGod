using System.Collections.Generic;
using UnityEngine;
using HOG.Grid;
using UnityEngine.Events;

namespace HOG.Villager
{
    public class VillagerLocomotion : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float m_MoveSpeed = 3f;
        public float m_StopDistance = 0.1f;
        public float m_RotationSpeed = 5f;
        public bool m_UsePhysicsMovement = true;

        [Header("Pathfinding Behavior")]
        public bool m_UseDynamicRepathing = false;
        public float m_RepathRate = 1f;

        private bool m_HasFiredDestinationEvent = false;
        public UnityAction OnDestinationReached;

        private VillagerBrain m_Brain;
        private Rigidbody m_Rigidbody;
        private Queue<Vector3> m_CurrentPath = new Queue<Vector3>();
        private Vector3? m_CurrentTarget;
        private Vector3 m_TargetDestination;

        private bool m_IsMoving = false;
        public bool IsMoving => m_IsMoving;

        private float m_RepathTimer;

        public void Initialize()
        {
            m_Brain = GetComponentInParent<VillagerBrain>();
            m_Rigidbody = m_Brain.GetRigidbody();
        }

        private void FixedUpdate()
        {
            if (!m_IsMoving)
                return;

            if (m_CurrentTarget == null && m_CurrentPath.Count > 0)
            {
                m_CurrentTarget = m_CurrentPath.Dequeue();
            }

            if (m_CurrentTarget == null)
                return;

            if (m_UseDynamicRepathing)
            {
                m_RepathTimer += Time.fixedDeltaTime;
                if (m_RepathTimer >= m_RepathRate)
                {
                    m_RepathTimer = 0f;
                    RecalculatePath();
                    return;
                }
            }

            Vector3 direction = m_CurrentTarget.Value - transform.position;
            direction.y = 0f;
            float distance = direction.magnitude;

            if (distance < m_StopDistance)
            {
                if (!m_HasFiredDestinationEvent && m_CurrentPath.Count == 0)
                {
                    StopMovement();
                    m_HasFiredDestinationEvent = true;
                    OnDestinationReached?.Invoke();
                    return;
                }

                if (m_CurrentPath.Count > 0)
                {
                    m_CurrentTarget = m_CurrentPath.Dequeue();
                }

                return;
            }

            Vector3 moveDir = direction.normalized;

            if (moveDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                m_Brain.transform.rotation = Quaternion.Slerp(m_Brain.transform.rotation, targetRotation, Time.fixedDeltaTime * m_RotationSpeed);
            }

            if (m_UsePhysicsMovement)
            {
                m_Rigidbody.AddForce(moveDir * m_MoveSpeed, ForceMode.Acceleration);
            }
            else
            {
                Vector3 newPos = m_Rigidbody.position + moveDir * m_MoveSpeed * Time.fixedDeltaTime;
                m_Rigidbody.MovePosition(newPos);
            }
        }

        public void SetDestination(Vector3 target)
        {
            m_HasFiredDestinationEvent = false;
            m_TargetDestination = target;

            Vector3 startPos = new Vector3(m_Brain.transform.position.x, 0, m_Brain.transform.position.z);
            Vector3 startPosRounded = new Vector3(Mathf.RoundToInt(startPos.x), 0, Mathf.RoundToInt(startPos.z));

            GridNode startNode = GridManager.Instance.GetNodeFromWorld(startPosRounded);
            GridNode targetNode = GridManager.Instance.GetNodeFromWorld(target);

            List<Vector3> path = AStarPathfinder.FindPath(startNode.m_WorldPos, targetNode.m_WorldPos, GridManager.Instance, true, true);

            if (path == null || path.Count == 0)
            {
                Debug.LogWarning("No path to target.");
                m_IsMoving = false;
                return;
            }

            //List<Vector3> cleanPath = CleanPathStart(path, startPosRounded);

            m_CurrentPath = new Queue<Vector3>(path);
            m_CurrentTarget = null;
            m_IsMoving = true;
            m_RepathTimer = 0f;

            GridManager.Instance.DebugVisualizer.SetPath(m_Brain.GetInstanceID().ToString() , path);
        }
        
        public void StopMovement()
        {
            GridManager.Instance.DebugVisualizer.ClearPath(m_Brain.GetInstanceID().ToString());
            m_CurrentPath.Clear();
            m_IsMoving = false;
            m_Rigidbody.linearVelocity = Vector3.zero;
        }

        public void StartSprint()
        {
            m_MoveSpeed = 1f;
        }

        public void StopSprint()
        {
            m_MoveSpeed = 0.5f;
        }

        public void PauseMovement()
        {
            m_IsMoving = false;
        }

        public void ResumeMovement()
        {
            m_IsMoving = true;
        }

        private void RecalculatePath()
        {
            Vector3 startPos = new Vector3(m_Brain.transform.position.x, 0, m_Brain.transform.position.z);

            var path = AStarPathfinder.FindPath(startPos, m_TargetDestination, GridManager.Instance, true, true);
            if (path == null || path.Count == 0)
            {
                StopMovement();
                return;
            }

            m_CurrentPath = new Queue<Vector3>(path);
            m_CurrentTarget = m_CurrentPath.Dequeue();

            //GridManager.Instance.DebugVisualizer.SetPath(path);
        }

        private List<Vector3> CleanPath(List<Vector3> path, Vector3 start,Vector3 target)
        {
            if (path.Count > 1 && GridManager.Instance.IsSameNode(path[0], start))
            {
                path.RemoveAt(0);
            }

            if (path.Count > 1 && GridManager.Instance.IsSameNode(path[^1], target))
            {
                path.RemoveAt(path.Count - 1);
                path.Add(target);
            }

            return path;
        }

        private List<Vector3> CleanPathStart(List<Vector3> path, Vector3 start)
        {
            if (path.Count > 1 && GridManager.Instance.IsSameNode(path[0], start))
            {
                path.RemoveAt(0);
            }

            return path;
        }

        private void CleanPathEnd(List<Vector3> path, Vector3 target)
        {
            if (path.Count > 1 && GridManager.Instance.IsSameNode(path[^1], target))
            {
                path.RemoveAt(path.Count - 1);
                path.Add(target);
            }
        }
    }
}
