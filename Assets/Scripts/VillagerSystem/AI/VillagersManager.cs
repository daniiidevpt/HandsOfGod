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

        public VillagerBrain FindClosestAvailableVillager(Vector3 position, VillagerBrain requester)
        {
            VillagerBrain closest = null;
            float minDist = float.MaxValue;

            foreach (var villager in m_Villagers)
            {
                if (villager == requester || villager.IsBusy || villager.GetStateMachine().CurrentState is TalkState)
                    continue;

                float dist = Vector3.Distance(position, villager.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = villager;
                }
            }

            return closest;
        }
    }
}