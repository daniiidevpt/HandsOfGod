using UnityEngine;

namespace HOG.Resources
{
    public class Wheat : Resource
    {
        private void Awake() => m_ResourceType = ResourceType.Wheat;

        public override ResourceCollectionInfo Collect()
        {
            ResourceCollectionInfo info = base.Collect();
            Destroy(gameObject);
            return info;
        }
    }
}
