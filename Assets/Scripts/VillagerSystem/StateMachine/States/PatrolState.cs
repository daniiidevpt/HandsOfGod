using System.Collections.Generic;
using UnityEngine;
using HOG.Grid;
using System.Collections;

namespace HOG.Villager
{
    public class PatrolState : BaseState
    {
        private List<Vector3> m_Waypoints = new List<Vector3>();
        private int m_CurrentWaypointIndex = 0;
        private int m_LastWaypointIndex = 0;

        private Coroutine m_ChooseWayointRoutine;

        public PatrolState(VillagerBrain villagerBrain, string stateName = null) : base(villagerBrain, stateName) 
        {
            for (int i = 0; i < villagerBrain.Waypoints.Count; i++)
            {
                Vector3 waypointPos = villagerBrain.Waypoints[i].transform.position;
                m_Waypoints.Add(waypointPos);
            }
        }

        public override void Enter()
        {
            base.Enter();

            m_CurrentWaypointIndex = 0;

            if (m_LastWaypointIndex <= m_Waypoints.Count)
            {
                m_LastWaypointIndex = 0;
            }

            m_Brain.GetLocomotion().OnDestinationReached += OnDestinationReached;

            MoveToNextWaypoint();
        }

        public override void Exit()
        {
            base.Exit();

            if (m_ChooseWayointRoutine != null)
            {
                m_Brain.StopCoroutine(m_ChooseWayointRoutine);
            }

            m_Brain.GetLocomotion().StopMovement();
            m_Brain.GetLocomotion().OnDestinationReached -= OnDestinationReached;
        }

        private void MoveToNextWaypoint()
        {
            m_ChooseWayointRoutine = m_Brain.StartCoroutine(MoveToNextWaypointWithDelay());
        }

        private IEnumerator MoveToNextWaypointWithDelay()
        {
            yield return new WaitForSeconds(1);

            if (m_Waypoints == null || m_Waypoints.Count == 0)
                yield break;

            int nextIndex;
            do
            {
                nextIndex = Random.Range(0, m_Waypoints.Count);
            } while (nextIndex == m_LastWaypointIndex && m_Waypoints.Count > 1);

            m_CurrentWaypointIndex = nextIndex;
            m_LastWaypointIndex = m_CurrentWaypointIndex;

            Vector3 nextWaypoint = m_Waypoints[m_CurrentWaypointIndex];
            GridNode node = GridManager.Instance.GetNodeFromWorld(nextWaypoint);

            if (node == null || !node.m_Walkable || node.m_IsPathBlocked)
            {
                Debug.LogWarning($"Waypoint at index {m_CurrentWaypointIndex} is not walkable. Skipping.");
                MoveToNextWaypoint();
                yield break;
            }

            m_Brain.GetLocomotion().SetDestination(nextWaypoint);
        }

        private void OnDestinationReached()
        {
            MoveToNextWaypoint();
        }
    }
}
