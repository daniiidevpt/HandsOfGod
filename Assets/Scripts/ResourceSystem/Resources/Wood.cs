using System.Collections;
using UnityEngine;

namespace HOG.Resources
{
    public class Wood : Resource, IBurnable
    {
        [Header("Tree Visuals")]
        [SerializeField] private GameObject m_TreeVisuals;
        private Collider m_TreeCollider;
        private int m_GrowbackTime;

        [Header("Tree VFX")]
        [SerializeField] private GameObject m_CollectVFX;

        [Header("Fire VFX")]
        [SerializeField] private GameObject m_FireVFX;
        [SerializeField] private int m_BurnTime = 3;
        private bool m_IsBurning;

        public bool IsBurning => m_IsBurning;

        private void Awake()
        {
            m_ResourceType = ResourceType.Wood;

            m_TreeCollider = GetComponent<Collider>();
            m_GrowbackTime = Random.Range(5, 10);
        }

        public override ResourceCollectionInfo Collect()
        {
            ResourceCollectionInfo info = base.Collect();

            GameObject VFXInst = Instantiate(m_CollectVFX, transform.position + Vector3.up, Quaternion.identity);
            Destroy(VFXInst, 1f);

            m_TreeVisuals.SetActive(false);
            m_TreeCollider.enabled = false;

            StartCoroutine(RegrowTree());

            return info;
        }

        private IEnumerator RegrowTree()
        {
            yield return new WaitForSeconds(m_GrowbackTime);

            m_TreeVisuals.SetActive(true);
            m_TreeCollider.enabled = true;

            m_IsCollected = false;
        }

        public void Ignite()
        {
            if (m_IsBurning || m_IsCollected) return;

            m_IsBurning = true;
            GameObject fire = Instantiate(m_FireVFX, transform.position + Vector3.up, Quaternion.identity);
            Destroy(fire, m_BurnTime);

            StartCoroutine(BurnUp());
        }

        private IEnumerator BurnUp()
        {
            yield return new WaitForSeconds(m_BurnTime);
            Collect();
        }
    }
}
