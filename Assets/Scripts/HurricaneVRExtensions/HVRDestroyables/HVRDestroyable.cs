using UnityEngine;

public class HVRDestroyable : MonoBehaviour
{
    [SerializeField] protected LayerMask m_Layer;

    protected virtual void OnCollisionEnter(Collision collision)
    {
        //Debug.Log($"Collided with:{collision.gameObject.name}");
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"Triggered with:{other.gameObject.name}");
    }
}
