using System.Collections;
using UnityEngine;

namespace HOG.Resources
{
    public class Wood : Resource
    {
        [Header("Tree VFX")]
        [SerializeField] private GameObject m_CollectVFX;

        private void Awake() => m_ResourceType = ResourceType.Wood;

        public override ResourceCollectionInfo Collect()
        {
            ResourceCollectionInfo info = base.Collect();

            GameObject VFXInst = Instantiate(m_CollectVFX, transform.position + Vector3.up, Quaternion.identity);
            Destroy(VFXInst, 1f);

            Destroy(this.gameObject);

            return info;
        }
    }
}
