using HOG.Building;
using HOG.Resources;
using UnityEngine;

namespace HOG.Villager
{
    public class BuildingState : BaseState
    {
        private Coroutine m_SwingRoutine;

        public BuildingState(VillagerBrain brain, string stateName = null) : base(brain, stateName) { }

        public void SetContext(StateContext context)
        {
            m_Context = context;
        }

        public override void Enter()
        {
            base.Enter();

            m_Brain.IsBusy = true;

            if (m_Context == null || m_Context.TargetBuilding == null)
            {
                Debug.LogWarning("No target building in context.");
                m_Brain.GetStateMachine().ChangeState(m_Brain.PatrolState);
                return;
            }

            var destination = m_Context.TargetBuilding.BuildingPosition.position;

            m_Brain.GetLocomotion().StopMovement();
            m_Brain.GetLocomotion().SetDestination(destination);
            m_Brain.GetLocomotion().OnDestinationReached += OnDestinationReached;
        }

        public override void Exit()
        {
            base.Exit();

            m_Brain.IsBusy = false;
            m_Context = null;

            if (m_SwingRoutine != null)
            {
                m_Brain.StopCoroutine(m_SwingRoutine);
                m_Brain.DisableTools();
                m_SwingRoutine = null;
            }

            m_Brain.GetLocomotion().OnDestinationReached -= OnDestinationReached;
        }

        private void OnDestinationReached()
        {
            if (m_Context?.TargetBuilding == null)
            {
                Debug.LogWarning("No building on arrival");
                m_Brain.GetStateMachine().ChangeState(m_Brain.PatrolState);
                return;
            }

            // YES IT WILL HAMMER FOR 5 MINUTES IF IT REACHES THAT POINT, WHY?
            // BECAUSE I FEEL LIKE IT :D
            m_SwingRoutine = m_Brain.StartCoroutine(m_Brain.PlayHammerSwing(300f));
        }
    }
}
