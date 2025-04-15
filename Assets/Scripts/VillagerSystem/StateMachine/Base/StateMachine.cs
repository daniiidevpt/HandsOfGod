using System.Collections.Generic;
using System.Linq;

namespace HOG.Villager
{
    public class StateMachine
    {
        private IState m_CurrentState;
        private List<StateTransition> m_CurrentTransitions = new();
        private Dictionary<IState, List<StateTransition>> m_Transitions = new();
        private Dictionary<string, IState> m_EventTransitions = new();

        public IState CurrentState => m_CurrentState;

        public void Update()
        {
            var transition = m_CurrentTransitions.FirstOrDefault(t => t.m_Condition());
            if (transition != null)
            {
                ChangeState(transition.m_TargetState);
            }

            m_CurrentState?.Update();
        }

        public void ChangeState(IState newState)
        {
            if (newState == m_CurrentState) return;

            m_CurrentState?.Exit();
            m_CurrentState = newState;

            m_Transitions.TryGetValue(m_CurrentState, out m_CurrentTransitions);
            m_CurrentTransitions ??= new List<StateTransition>();

            m_CurrentState.Enter();
        }

        public void AddTransition(IState from, IState to, System.Func<bool> condition)
        {
            if (!m_Transitions.ContainsKey(from))
            {
                m_Transitions[from] = new List<StateTransition>();
            }

            m_Transitions[from].Add(new StateTransition(to, condition));
        }

        public void RegisterEventTransition(string eventName, IState toState)
        {
            m_EventTransitions[eventName] = toState;
        }

        public void TriggerEvent(string eventName)
        {
            if (m_EventTransitions.TryGetValue(eventName, out var newState))
            {
                ChangeState(newState);
            }
        }
    }
}
