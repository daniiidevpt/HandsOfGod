using UnityEngine;

namespace HOG.Villager
{
    public class PanicState : BaseState
    {
        private Vector3 m_EscapePosition;

        public PanicState(VillagerBrain villagerBrain, string stateName = null) : base(villagerBrain, stateName) { }

        public override void Enter()
        {
            base.Enter();

            m_Brain.GetLocomotion().StopMovement();
            m_Brain.GetLocomotion().OnDestinationReached += OnDestinationReached;

            m_EscapePosition = GetFarthestWaypoint();
            m_Brain.GetLocomotion().StartSprint();
            m_Brain.GetLocomotion().SetDestination(m_EscapePosition);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();

            m_Brain.GetLocomotion().StopSprint();
            m_Brain.GetLocomotion().OnDestinationReached -= OnDestinationReached;
        }

        private void OnDestinationReached()
        {
            m_Brain.GetStateMachine().ChangeState(m_Brain.PatrolState);
        }

        private Vector3 GetFarthestWaypoint()
        {
            Vector3 currentPos = m_Brain.transform.position;
            Vector3 farthest = currentPos;
            float maxDistance = 0f;

            foreach (var waypoint in m_Brain.Waypoints)
            {
                float dist = Vector3.Distance(currentPos, waypoint.position);
                if (dist > maxDistance)
                {
                    maxDistance = dist;
                    farthest = waypoint.position;
                }
            }

            return farthest;
        }
    }
}
