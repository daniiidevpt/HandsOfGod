using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

namespace HOG.Resources
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        private BehaviorGraphAgent m_Graph;
        private Dictionary<ResourceType, int> m_Storage = new Dictionary<ResourceType, int>();

        [SerializeField] private OnResourcesChanged OnResourcesChanged;

        protected override void Awake()
        {
            base.Awake();

            m_Graph = GetComponentInParent<BehaviorGraphAgent>();
        }

        private void Start()
        {
            AddResource(new ResourceCollectionInfo(ResourceType.Wood, 5));
            AddResource(new ResourceCollectionInfo(ResourceType.Rock, 15));
        }

        public void AddResource(ResourceCollectionInfo info)
        {
            if (m_Storage.ContainsKey(info.m_ResourceType))
            {
                m_Storage[info.m_ResourceType] += info.m_Amount;

                OnResourcesChanged.SendEventMessage();

                if (info.m_ResourceType == ResourceType.Wood)
                {
                    m_Graph.SetVariableValue("GLOBAL_Wood", m_Storage[ResourceType.Wood]);

                    if (m_Graph.GetVariable("GLOBAL_Wood", out BlackboardVariable woodAmount))
                        Debug.Log($"GLOBAL WOOD: {woodAmount.ObjectValue}");
                }

                if (info.m_ResourceType == ResourceType.Rock)
                {
                    m_Graph.SetVariableValue("GLOBAL_Rock", m_Storage[ResourceType.Rock]);

                    if (m_Graph.GetVariable("GLOBAL_Rock", out BlackboardVariable rockAmount))
                        Debug.Log($"GLOBAL ROCK: {rockAmount.ObjectValue}");
                }
            }
            else
            {
                m_Storage[info.m_ResourceType] = info.m_Amount;
            }

            //Debug.Log($"Village received {info.m_Amount} {info.m_ResourceType}. Total: {m_Storage[info.m_ResourceType]}");
        }

        public bool RemoveResource(ResourceType type, int amount)
        {
            if (m_Storage.ContainsKey(type) && m_Storage[type] >= amount)
            {
                m_Storage[type] -= amount;
                //Debug.Log($"Used {amount} {type}. Remaining: {m_Storage[type]}");

                OnResourcesChanged.SendEventMessage();

                if (type == ResourceType.Wood)
                {
                    m_Graph.SetVariableValue("GLOBAL_Wood", m_Storage[ResourceType.Wood]);

                    if (m_Graph.GetVariable("GLOBAL_Wood", out BlackboardVariable woodAmount))
                        Debug.Log($"GLOBAL WOOD: {woodAmount.ObjectValue}");
                }

                if (type == ResourceType.Rock)
                {
                    m_Graph.SetVariableValue("GLOBAL_Rock", m_Storage[ResourceType.Rock]);

                    if (m_Graph.GetVariable("GLOBAL_Rock", out BlackboardVariable rockAmount))
                        Debug.Log($"GLOBAL ROCK: {rockAmount.ObjectValue}");
                }

                return true;
            }
            else
            {
                //Debug.Log($"Not enough {type} in the village!");
                return false;
            }
        }

        public int GetResourceAmount(ResourceType type)
        {
            return m_Storage.ContainsKey(type) ? m_Storage[type] : 0;
        }
    }
}
