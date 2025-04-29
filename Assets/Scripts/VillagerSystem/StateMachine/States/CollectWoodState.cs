using HOG.Resources;
using UnityEngine;

namespace HOG.Villager
{
    public class CollectWoodState : BaseState
    {
        private Coroutine m_SwingRoutine;
        private Coroutine m_CollectRoutine;

        public CollectWoodState(VillagerBrain brain, string stateName = null) : base(brain, stateName) { }

        public void SetContext(StateContext context)
        {
            m_Context = context;
        }

        public override void Enter()
        {
            base.Enter();

            m_Brain.IsBusy = true;

            if (m_Context == null || m_Context.Target == null || m_Context.TargetResource == null)
            {
                Debug.LogWarning("Context or TargetResource is null. Returning to Patrol.");
                m_Brain.GetStateMachine().ChangeState(m_Brain.PatrolState);
                return;
            }

            var destination = m_Context.Target.position;

            m_Brain.GetLocomotion().StopMovement();
            m_Brain.GetLocomotion().SetDestination(destination);
            m_Brain.GetLocomotion().OnDestinationReached += OnDestinationReached;
        }

        public override void Update()
        {
            base.Update();

            if (m_Context.TargetResource.IsCollected)
            {
                m_Brain.GetStateMachine().ChangeState(m_Brain.PatrolState);
            }
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

            if (m_CollectRoutine != null)
            {
                m_Brain.StopCoroutine(m_CollectRoutine);
                m_CollectRoutine = null;
            }

            m_Brain.GetLocomotion().OnDestinationReached -= OnDestinationReached;
        }

        private void OnDestinationReached()
        {
            if (m_Context.TargetResource == null)
            {
                Debug.LogWarning("Resource missing on arrival.");
                m_Brain.GetStateMachine().ChangeState(m_Brain.PatrolState);
                return;
            }

            m_SwingRoutine = m_Brain.StartCoroutine(m_Brain.PlayHammerSwing(m_Context.TargetResource.TimeToCollect));
            m_CollectRoutine = m_Brain.StartCoroutine(m_Context.TargetResource.StartCollection());
        }
    }
}
