using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HOG.Villager
{
    public class VillagersManager : Singleton<VillagersManager>
    {
        [Header("Villagers")]
        [SerializeField] private List<VillagerBrain> m_Villagers = new List<VillagerBrain>();

        protected override void Awake()
        {
            base.Awake();

            m_Villagers = FindObjectsByType<VillagerBrain>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
        }

        public VillagerBrain AssignVillagerToBuilding(Vector3 housePosition)
        {
            int rnd = Random.Range(0, m_Villagers.Count);

            VillagerBrain villager = m_Villagers[rnd];
            villager.GetStateMachine().ChangeState(new BuildingState(villager, null, housePosition));

            return villager;
        }

        public void UnassignVillagerFromBuilding(VillagerBrain villager)
        {
            if (villager.GetStateMachine().CurrentState is not BuildingState)
            {
                Debug.LogError($"{villager.name} was not building when it was supposed to be");
            }

            villager.GetStateMachine().ChangeState(villager.PatrolState);
        }
    }
}