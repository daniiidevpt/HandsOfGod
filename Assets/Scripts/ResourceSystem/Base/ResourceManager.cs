using System.Collections.Generic;
using UnityEngine;

namespace HOG.Resources
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        private Dictionary<ResourceType, int> m_Storage = new Dictionary<ResourceType, int>();

        private void Start()
        {
            AddResource(new ResourceCollectionInfo(ResourceType.Wood, 15));
            AddResource(new ResourceCollectionInfo(ResourceType.Rock, 15));
        }

        public void AddResource(ResourceCollectionInfo info)
        {
            if (m_Storage.ContainsKey(info.m_ResourceType))
            {
                m_Storage[info.m_ResourceType] += info.m_Amount;
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
