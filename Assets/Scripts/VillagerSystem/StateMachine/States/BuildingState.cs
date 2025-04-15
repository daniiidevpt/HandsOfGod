using UnityEngine;

namespace HOG.Villager
{
    public class BuildingState : BaseState
    {
        private Vector3 m_HousePosition;

        public BuildingState(VillagerBrain villagerBrain, string stateName = null, Vector3? housePosition = null) : base(villagerBrain, stateName)
        {
            m_HousePosition = housePosition.Value;
        }

        public override void Enter()
        {
            base.Enter();

            m_Brain.GetLocomotion().StopMovement();

            m_Brain.GetLocomotion().SetDestination(m_HousePosition);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
