using UnityEngine;

namespace HOG.Villager
{
    public class BaseState : IState
    {
        protected string m_StateName;
        protected VillagerBrain m_Brain;
        protected StateContext m_Context;

        public string StateName => m_StateName;

        public BaseState(VillagerBrain villagerBrain, string stateName = null)
        {
            m_Brain = villagerBrain;

            m_StateName = stateName ?? GetType().Name;
        }

        public virtual void Enter()
        {
            Debug.Log($"<color=green>[ENTER]</color> {m_StateName}");
        }

        public virtual void Update()
        {
            // 
        }

        public virtual void Exit()
        {
            Debug.Log($"<color=red>[EXIT]</color> {m_StateName}");
        }
    }
}
