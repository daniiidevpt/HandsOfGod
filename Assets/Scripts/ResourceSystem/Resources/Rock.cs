using UnityEngine;

namespace HOG.Resources
{
    public class Rock : Resource
    {
        [Header("Rock VFX")]
        [SerializeField] private GameObject m_CollectVFX;

        private void Awake() => m_ResourceType = ResourceType.Rock;

        public override ResourceCollectionInfo Collect()
        {
            ResourceCollectionInfo info = base.Collect();

            GameObject VFXInst = Instantiate(m_CollectVFX, transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
            Destroy(VFXInst, 1f);

            Destroy(gameObject);

            return info;
        }
    }
}
