using UnityEngine;

namespace HOG.Villager
{
    public interface IUtilityState : IState
    {
        float GetScore(Vector3 dropPosition);
    }

}