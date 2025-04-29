using System.Collections;
using UnityEngine;

namespace HOG.Villager
{
    public class PanicState : BaseState
    {
        private Vector3 m_EscapePosition;
        private Coroutine m_PanicRoutine;

        public PanicState(VillagerBrain villagerBrain, string stateName = null) : base(villagerBrain, stateName) { }

        public override void Enter()
        {
            base.Enter();

            m_Brain.GetLocomotion().StopMovement();
            m_Brain.GetLocomotion().StartSprint();

            m_PanicRoutine = m_Brain.StartCoroutine(PanicRoutine());

            m_Brain.GetLocomotion().OnDestinationReached += OnDestinationReached;
        }

        public override void Exit()
        {
            base.Exit();

            m_Brain.GetLocomotion().StopSprint();

            if (m_PanicRoutine != null)
            {
                m_Brain.StopCoroutine(m_PanicRoutine);
                m_PanicRoutine = null;
            }

            m_Brain.GetLocomotion().OnDestinationReached -= OnDestinationReached;
        }

        private void OnDestinationReached()
        {
            m_Brain.GetStateMachine().ChangeState(m_Brain.PatrolState);
        }

        private IEnumerator PanicRoutine()
        {
            float panicDuration = Random.Range(2f, 5f);
            float elapsedTime = 0f;

            while (elapsedTime < panicDuration)
            {
                Vector3 randomDir = Random.insideUnitSphere * 5f;
                randomDir.y = 0;
                Vector3 randomTarget = m_Brain.transform.position + randomDir;

                m_Brain.GetLocomotion().SetDestination(randomTarget);

                yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
                elapsedTime += Random.Range(0.5f, 1.5f);
            }

            m_EscapePosition = GetFarthestWaypoint();

            m_Brain.GetLocomotion().SetDestination(m_EscapePosition);
        }


        private Vector3 GetFarthestWaypoint()
        {
            Vector3 currentPos = m_Brain.transform.position;
            Vector3 farthest = currentPos;
            float maxDistance = 0f;

            foreach (var waypoint in m_Brain.Waypoints)
            {
                if (waypoint == null) continue;

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
