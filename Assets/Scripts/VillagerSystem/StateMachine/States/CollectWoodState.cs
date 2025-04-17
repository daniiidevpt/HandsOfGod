using HOG.Resources;
using UnityEngine;

namespace HOG.Villager
{
    public class CollectWoodState : BaseState, IUtilityState
    {
        private Resource m_Resource;

        private Coroutine m_SwingRoutine;
        private Coroutine m_CollectRoutine;

        public CollectWoodState(VillagerBrain villagerBrain, string stateName = null) : base(villagerBrain, stateName) { }

        public float GetScore(Vector3 villagerPosition)
        {
            var wood = m_Brain.GetSensor().GetClosestTarget("Resource_Wood");
            if (wood == null) return 0f;

            float distance = Vector3.Distance(villagerPosition, wood.position);
            m_Resource = wood.GetComponent<Wood>();

            return 1f / Mathf.Max(distance, 0.1f);
        }

        public override void Enter()
        {
            base.Enter();

            if (m_Resource == null)
            {
                var wood = m_Brain.GetSensor().GetClosestTarget("Resource_Wood");
                if (wood != null)
                {
                    m_Resource = wood.GetComponent<Wood>();
                }
            }

            if (m_Resource == null)
            {
                Debug.LogWarning("Tree was null");
                m_Brain.GetStateMachine().ChangeState(m_Brain.PatrolState);
                return;
            }

            m_Brain.GetLocomotion().StopMovement();
            m_Brain.GetLocomotion().OnDestinationReached += OnDestinationReached;
            m_Brain.GetLocomotion().SetDestination(m_Resource.transform.position);
        }

        public override void Update()
        {
            base.Update();

            if (m_Resource.IsCollected)
            {
                m_Brain.GetStateMachine().ChangeState(m_Brain.PatrolState);
                return;
            }
        }

        public override void Exit()
        {
            base.Exit();

            if (m_SwingRoutine != null)
            {
                m_Brain.StopCoroutine(m_SwingRoutine);
                m_SwingRoutine = null;
            }

            if (m_CollectRoutine != null)
            {
                m_Brain.StopCoroutine(m_CollectRoutine);
                m_CollectRoutine = null;
            }

            m_Brain.GetLocomotion().OnDestinationReached -= OnDestinationReached;
        }

        private void OnDestinationReached()
        {
            m_SwingRoutine = m_Brain.StartCoroutine(m_Brain.PlayVillagerSwing(m_Brain.Hammer, m_Brain.VillagerSwingAnim, m_Resource.TimeToCollect));
            m_CollectRoutine = m_Brain.StartCoroutine(m_Resource.StartCollection());
        }
    }
}
