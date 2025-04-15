using System;

namespace HOG.Villager
{
    public class StateTransition
    {
        public Func<bool> m_Condition;
        public IState m_TargetState;

        public StateTransition(IState targetState, Func<bool> condition)
        {
            m_TargetState = targetState;
            m_Condition = condition;
        }
    }
}
