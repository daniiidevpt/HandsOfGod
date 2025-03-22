using System.Collections;
using UnityEngine;

namespace HOG.Resources
{
    public class Wood : Resource
    {
        [Header("Tree Settings")]
        [SerializeField] private GameObject m_Tree;
        [SerializeField] private Collider m_TreeCollider;
        [SerializeField] private GameObject m_TreeStump;
        [SerializeField] private Collider m_TreeStumpCollider;
        [SerializeField] private float m_RegrowTime = 30f;

        [Header("Tree VFX")]
        [SerializeField] private GameObject m_CollectVFX;

        // TODO: REMOVE UNUSED COROUTINE REFERENCE TO SAVE MEMORY IF NEEDED
        private Coroutine m_GrowRoutine = null;
        private bool m_IsRegrowing = false;

        public bool IsRegrowing => m_IsRegrowing;

        private void Awake() => m_ResourceType = ResourceType.Wood;

        public override ResourceCollectionInfo Collect()
        {
            if (m_IsRegrowing)
            {
                Debug.LogError($"{gameObject.name} is already regrowing");
                return new ResourceCollectionInfo(ResourceType.Null, 0);
            }

            ResourceCollectionInfo info = base.Collect();

            StartGrowRoutine();
            this.gameObject.tag = "Untagged";

            m_Tree.SetActive(false);
            m_TreeCollider.enabled = false;

            m_TreeStump.SetActive(true);
            m_TreeStumpCollider.enabled = true;

            GameObject VFXInst = Instantiate(m_CollectVFX, transform.position + Vector3.up, Quaternion.identity);
            Destroy(VFXInst, 1f);

            return info;
        }

        private void StartGrowRoutine()
        {
            if (m_IsRegrowing) return;

            m_IsRegrowing = true;
            m_GrowRoutine = StartCoroutine(Regrow());
        }

        private IEnumerator Regrow()
        {
            yield return new WaitForSeconds(m_RegrowTime);

            m_IsRegrowing = false;
            m_IsCollected = false;

            this.gameObject.tag = "Resource_Wood";
            m_TreeStump.SetActive(false);
            m_TreeStumpCollider.enabled = false;
            m_Tree.SetActive(true);
            m_TreeCollider.enabled = true;

            m_GrowRoutine = null;
        }
    }
}
