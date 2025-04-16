using HOG.Resources;
using UnityEngine;

namespace HOG.Villager
{
    public class CollectRockState : BaseState, IUtilityState
    {
        private Resource m_Resource;

        public CollectRockState(VillagerBrain villagerBrain, string stateName = null) : base(villagerBrain, stateName) { }

        public float GetScore(Vector3 villagerPosition)
        {
            var rock = m_Brain.GetSensor().GetClosestTarget("Resource_Rock");
            if (rock == null) return 0f;

            float distance = Vector3.Distance(villagerPosition, rock.position);
            m_Resource = rock.GetComponent<Rock>();

            return 1f / Mathf.Max(distance, 0.1f);
        }

        public override void Enter()
        {
            base.Enter();

            if (m_Resource == null)
            {
                var rock = m_Brain.GetSensor().GetClosestTarget("Resource_Rock");
                if ( rock != null)
                {
                    m_Resource = rock.GetComponent<Rock>();
                }
            }

            if (m_Resource == null)
            {
                Debug.LogWarning("Rock was null");
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