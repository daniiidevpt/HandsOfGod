using System.Collections;
using UnityEngine;

namespace HOG.Villager
{
    public class TalkState : BaseState
    {
        private VillagerBrain m_Partner;
        private Vector3 m_MeetingPoint;
        private Coroutine m_TalkRoutine;

        public TalkState(VillagerBrain brain, string name = null) : base(brain, name) { }

        public void SetContext(VillagerBrain partner, Vector3 meetingPoint)
        {
            m_Partner = partner;
            m_MeetingPoint = meetingPoint;
        }

        public override void Enter()
        {
            base.Enter();

            m_Brain.IsBusy = true;

            m_Brain.GetLocomotion().SetDestination(m_MeetingPoint);
            m_Brain.GetLocomotion().OnDestinationReached += OnDestinationReached;
        }

        public override void Exit()
        {
            base.Exit();

            m_Brain.IsBusy = false;
            m_Partner = null;

            if (m_TalkRoutine != null)
            {
                m_Brain.StopCoroutine(m_TalkRoutine);
            }

            m_Brain.GetLocomotion().OnDestinationReached -= OnDestinationReached;
        }

        private void OnDestinationReached()
        {
            m_TalkRoutine = m_Brain.StartCoroutine(TalkRoutine());
        }

        private IEnumerator TalkRoutine()
        {
            yield return new WaitForSeconds(.5f);

            if (m_Partner != null)
            {
                Vector3 dir = (m_Partner.transform.position - m_Brain.transform.position).normalized;
                dir.y = 0;
                m_Brain.transform.rotation = Quaternion.LookRotation(dir);
            }

            m_Brain.PlayVillagerTalk();

            yield return new WaitForSeconds(5f);

            m_Brain.StopVillagerTalk();

            m_Brain.GetStateMachine().ChangeState(m_Brain.PatrolState);
        }
    }

}
