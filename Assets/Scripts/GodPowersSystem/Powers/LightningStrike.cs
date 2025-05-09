using HOG.Villager;
using UnityEngine;

namespace HOG.Effects
{
    public class LightningStrike : MonoBehaviour
    {
        [Header("Lightning Settings")]
        [SerializeField] private GameObject m_LightningVFX;
        [SerializeField] private float m_LightningRadius = 7f;
        [SerializeField] private LayerMask m_VillagerLayer;

        [Header("Ignition Settings")]
        [SerializeField] private float m_IgnitionRadius = 5f;
        [SerializeField] private LayerMask m_IgnitionTargetMask;

        [Header("Valid Collision Layers")]
        [SerializeField] private LayerMask m_CollisionMask;

        private void OnCollisionEnter(Collision collision)
        {
            if (((1 << collision.gameObject.layer) & m_CollisionMask) == 0)
                return;

            IgniteNearbyObjects();
            SetPanicState();

            Destroy(this.gameObject);
        }
        
        private void IgniteNearbyObjects()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, m_IgnitionRadius, m_IgnitionTargetMask);

            foreach (var hit in hits)
            {
                IBurnable burnable = hit.GetComponent<IBurnable>();
                if (burnable != null && !burnable.IsBurning)
                {
                    burnable.Ignite();
                    GameObject lightning = Instantiate(m_LightningVFX, hit.transform.position + Vector3.up, Quaternion.identity);
                    Destroy(lightning, 1f);
                }
            }
        }

        private void SetPanicState()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, m_LightningRadius, m_VillagerLayer);

            foreach (var hit in hits)
            {
                VillagerBrain villager = hit.GetComponent<VillagerBrain>();
                if (villager != null)
                {
                    villager.GetStateMachine().TriggerEvent("Panic");
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_IgnitionRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, m_LightningRadius);
        }
    }
}
