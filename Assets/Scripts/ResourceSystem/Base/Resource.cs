using System.Collections;
using UnityEngine;

namespace HOG.Resources
{
    public abstract class Resource : MonoBehaviour
    {
        [Header("Resource Settings")]
        [SerializeField] protected ResourceType m_ResourceType;
        [SerializeField] protected int m_ResourceAmount = 1;
        protected bool m_IsCollected = false;

        [Header("Durability Settings")]
        [SerializeField] protected int m_TimeToCollect = 3;

        public bool IsCollected => m_IsCollected;
        public int TimeToCollect => m_TimeToCollect;

        public virtual IEnumerator StartCollection()
        {
            yield return new WaitForSeconds(m_TimeToCollect);

            Collect();
        }

        public virtual ResourceCollectionInfo Collect()
        {
            if (m_IsCollected)
            {
                Debug.LogError($"{gameObject.name} already collected");
                return new ResourceCollectionInfo(ResourceType.Null, 0);
            }

            Debug.Log($"Collected {m_ResourceAmount} {m_ResourceType} from {gameObject.name}");
            m_IsCollected = true;

            ResourceCollectionInfo collectedResource = new ResourceCollectionInfo(m_ResourceType, m_ResourceAmount);
            ResourceManager.Instance.AddResource(collectedResource);

            return collectedResource;
        }

        public ResourceType GetResourceType() => m_ResourceType;
    }
}
