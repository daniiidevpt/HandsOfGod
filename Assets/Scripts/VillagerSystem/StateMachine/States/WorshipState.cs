using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace HOG.Villager
{
    public class WorshipState : BaseState
    {
        private Coroutine m_WorshipRoutine;
        private Vector3 m_Altar;

        public WorshipState(VillagerBrain villagerBrain, string stateName = null) : base(villagerBrain, stateName) { }

        public void SetContext(StateContext context)
        {
            m_Context = context;
        }

        public override void Enter()
        {
            base.Enter();

            m_Brain.IsBusy = true;

            if (m_Context == null)
            {
                m_Brain.GetStateMachine().ChangeState(m_Brain.PatrolState);
                return;
            }

            m_Altar = m_Context.Target.parent.transform.position;

            var destination = m_Context.Target.position;

            m_Brain.GetLocomotion().StopMovement();
            m_Brain.GetLocomotion().SetDestination(destination);
            m_Brain.GetLocomotion().OnDestinationReached += OnDestinationReached;
        }

        public override void Exit()
        {
            base.Exit();

            m_Brain.IsBusy = false;

            if (m_WorshipRoutine != null)
            {
                m_Brain.StopCoroutine(m_WorshipRoutine);
            }

            m_Brain.GetLocomotion().OnDestinationReached -= OnDestinationReached;
        }

        private void OnDestinationReached()
        {
            m_WorshipRoutine = m_Brain.StartCoroutine(WorshipRoutine());

            m_Brain.transform.LookAt(m_Altar, Vector3.up);
        }

        private IEnumerator WorshipRoutine()
        {
            yield return new WaitForSeconds(.5f);

            m_Brain.PlayVillagerWorkship();

            yield return new WaitForSeconds(5f);

            m_Brain.StopVillagerWorkship();

            m_Brain.GetStateMachine().ChangeState(m_Brain.PatrolState);
        }
    }
}