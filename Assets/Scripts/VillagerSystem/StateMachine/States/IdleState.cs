using UnityEngine;

namespace HOG.Villager
{
    public class IdleState : BaseState
    {
        public IdleState(VillagerBrain villagerBrain, string stateName = null) : base(villagerBrain, stateName) { }

        public override void Enter()
        {
            base.Enter();
            m_Brain.GetLocomotion().StopMovement();
        }

        public override void Update()
        {
            base.Update();

            Debug.Log("Idling");
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
