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

        private Coroutine m_ChooseWaypointRoutine;

        private Queue<int> m_RecentWaypoints = new Queue<int>();
        private const int m_RecentWaypointLimit = 5;

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

            MoveToNextWaypoint();

            m_Brain.GetLocomotion().OnDestinationReached += OnDestinationReached;
        }

        public override void Exit()
        {
            base.Exit();

            if (m_ChooseWaypointRoutine != null)
            {
                m_Brain.StopCoroutine(m_ChooseWaypointRoutine);
                m_ChooseWaypointRoutine = null;
            }

            m_Brain.GetLocomotion().StopMovement();
            m_Brain.GetLocomotion().OnDestinationReached -= OnDestinationReached;
        }

        private void MoveToNextWaypoint()
        {
            m_ChooseWaypointRoutine = m_Brain.StartCoroutine(MoveToNextWaypointWithDelay());
        }

        private IEnumerator MoveToNextWaypointWithDelay()
        {
            if (m_Waypoints == null || m_Waypoints.Count == 0)
                yield break;

            int nextIndex;
            int attempts = 0;
            const int maxAttempts = 10;

            do
            {
                nextIndex = Random.Range(0, m_Waypoints.Count);
                attempts++;
            }
            while (m_RecentWaypoints.Contains(nextIndex) && attempts < maxAttempts);

            m_CurrentWaypointIndex = nextIndex;

            if (m_RecentWaypoints.Count >= m_RecentWaypointLimit)
            {
                m_RecentWaypoints.Dequeue();
            }
            m_RecentWaypoints.Enqueue(m_CurrentWaypointIndex);

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
