using UnityEngine;
using HOG.Resources;

namespace HOG.Villager
{
    public class CollectState : BaseState
    {
        private ResourceType m_ResourceType;
        private Resource m_Resource;

        public CollectState(VillagerBrain villagerBrain, string stateName = null, ResourceType resourceType = ResourceType.Null) : base(villagerBrain, stateName) 
        { 
            m_ResourceType = resourceType;
        }

        public override void Enter()
        {
            base.Enter();

            m_Brain.GetLocomotion().StopMovement();

            m_Brain.GetLocomotion().OnDestinationReached += OnDestinationReached;

            switch (m_ResourceType)
            {
                case ResourceType.Wood:
                    var resourceWood = m_Brain.GetSensor().GetClosestTarget("Resource_Wood");
                    m_Resource = resourceWood.GetComponent<Wood>();
                    break;

                case ResourceType.Rock:
                    var resourceRock = m_Brain.GetSensor().GetClosestTarget("Resource_Rock");
                    m_Resource = resourceRock.GetComponent<Rock>();
                    break;

            }

            m_Brain.GetLocomotion().SetDestination(m_Resource.transform.position);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();

            m_Brain.GetLocomotion().OnDestinationReached -= OnDestinationReached;
        }

        private void OnDestinationReached()
        {
            m_Resource.Collect();

            m_Brain.GetStateMachine().ChangeState(m_Brain.PatrolState);
        }
    }
}
