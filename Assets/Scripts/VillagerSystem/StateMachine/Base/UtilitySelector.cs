using System.Collections.Generic;
using UnityEngine;

namespace HOG.Villager
{
    public class UtilitySelector
    {
        private readonly List<IUtilityState> m_UtilityStates = new();

        public void RegisterState(IUtilityState state)
        {
            if (!m_UtilityStates.Contains(state))
                m_UtilityStates.Add(state);
        }

        public IUtilityState GetBestState(Vector3 contextPosition)
        {
            float bestScore = float.MinValue;
            IUtilityState bestState = null;

            foreach (var state in m_UtilityStates)
            {
                float score = state.GetScore(contextPosition);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestState = state;
                }
            }

            return bestState;
        }
    }
}
