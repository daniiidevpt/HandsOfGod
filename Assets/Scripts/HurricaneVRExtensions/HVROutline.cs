using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using UnityEngine;

public class HVROutline : MonoBehaviour
{
    [SerializeField] private Material m_Outline;

    private HVRGrabbable m_Grabbable;
    private MeshRenderer[] m_Renderers;
    private Material[] m_Materials;

    private void Awake()
    {
        m_Grabbable = GetComponent<HVRGrabbable>();
        m_Renderers = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < m_Renderers.Length; i++)
        {
            m_Materials = m_Renderers[i].materials;
        }
    }

    private void OnEnable()
    {
        m_Grabbable.HoverEnter.AddListener(EnableOutline);
        m_Grabbable.HoverExit.AddListener(DisableOutline);
    }

    private void OnDisable()
    {
        m_Grabbable.HoverEnter.RemoveListener(EnableOutline);
        m_Grabbable.HoverExit.RemoveListener(DisableOutline);
    }

    private void EnableOutline(HVRGrabberBase grabberBase, HVRGrabbable grabbable)
    {
        Material[] newMaterials = new Material[m_Materials.Length + 1];
        m_Materials.CopyTo(newMaterials, 0);
        newMaterials[newMaterials.Length - 1] = m_Outline;

        for (int i = 0; i < m_Renderers.Length; i++)
        {
            m_Renderers[i].materials = newMaterials;
        }
    }

    private void DisableOutline(HVRGrabberBase grabberBase, HVRGrabbable grabbable)
    {
        for (int i = 0; i < m_Renderers.Length; i++)
        {
            m_Renderers[i].materials = m_Materials;
        }
    }
}
