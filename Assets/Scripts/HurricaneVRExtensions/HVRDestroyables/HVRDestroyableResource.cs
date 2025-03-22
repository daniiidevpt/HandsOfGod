using HOG.Resources;
using UnityEngine;

public class HVRDestroyableResource : HVRDestroyable
{
    public Resource m_Resource;

    private void Start()
    {
        m_Resource = GetComponent<Resource>();
        if (m_Resource == null)
        {
            Debug.LogError("Could not get the resource");
            return;
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (m_Resource.IsCollected) return;

        if (((1 << collision.gameObject.layer) & m_Layer) != 0)
        {
            m_Resource.Collect();
        }
    }
}
