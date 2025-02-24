using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    [SerializeField] private Vector3 m_MinRange = new Vector3(-5, 0, -5);
    [SerializeField] private Vector3 m_MaxRange = new Vector3(5, 0, 5);
    [SerializeField] private float m_Speed = 3f;
    [SerializeField] private float m_JumpForce = 5f;
    [SerializeField] private float m_JumpChance = 0.2f;
    [SerializeField] private Material m_Outline;

    private Rigidbody m_Rigidbody;
    private HVRGrabbable m_Grabbable;
    private MeshRenderer m_Renderer;
    private Material[] m_Materials;
    private Vector3 m_TargetPos;
    private bool m_IsGrounded = true;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Grabbable = GetComponent<HVRGrabbable>();
        m_Renderer = GetComponentInChildren<MeshRenderer>();
        m_Materials = m_Renderer.materials;

        SetNewTarget();
    }

    private void OnEnable()
    {
        m_Grabbable.HandGrabbed.AddListener(UnlockRotation);
        m_Grabbable.HandReleased.AddListener(LockRotation);

        m_Grabbable.HoverEnter.AddListener(EnableOutline);
        m_Grabbable.HoverExit.AddListener(DisableOutline);
    }

    private void OnDisable()
    {
        m_Grabbable.HandGrabbed.RemoveListener(UnlockRotation);
        m_Grabbable.HandReleased.RemoveListener(LockRotation);

        m_Grabbable.HoverEnter.RemoveListener(EnableOutline);
        m_Grabbable.HoverExit.RemoveListener(DisableOutline);
    }

    private void FixedUpdate()
    {
        if (m_Grabbable.IsHandGrabbed) return;
        MoveTowardsTarget();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            m_IsGrounded = true;

            Quaternion correctRotation = Quaternion.Euler(0f, 0f, 0f);
            m_Rigidbody.MoveRotation(correctRotation);
        }
    }

    private void LockRotation(HVRHandGrabber handGrabber, HVRGrabbable grabbable)
    {
        RigidbodyConstraints freezeX = RigidbodyConstraints.FreezeRotationX;
        RigidbodyConstraints freezeZ = RigidbodyConstraints.FreezeRotationZ;
        m_Rigidbody.constraints = freezeX | freezeZ;
    }

    private void UnlockRotation(HVRHandGrabber handGrabber, HVRGrabbable grabbable)
    {
        RigidbodyConstraints unfreeze = RigidbodyConstraints.None;
        m_Rigidbody.constraints = unfreeze;
    }

    private void EnableOutline(HVRGrabberBase grabberBase, HVRGrabbable grabbable)
    {
        Material[] newMaterials = new Material[m_Materials.Length + 1];
        m_Materials.CopyTo(newMaterials, 0);
        newMaterials[newMaterials.Length - 1] = m_Outline;
        m_Renderer.materials = newMaterials;
    }

    private void DisableOutline(HVRGrabberBase grabberBase, HVRGrabbable grabbable)
    {
        m_Renderer.materials = m_Materials;
    }

    private void SetNewTarget()
    {
        m_TargetPos = GetRandomPosition();
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(
            Random.Range(m_MinRange.x, m_MaxRange.x), 
            transform.position.y,
            Random.Range(m_MinRange.z, m_MaxRange.z)
        );
    }

    private void MoveTowardsTarget()
    {
        Vector3 direction = (m_TargetPos - m_Rigidbody.position).normalized;
        Vector3 movement = new Vector3(direction.x, 0, direction.z) * m_Speed * Time.fixedDeltaTime;

        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);

        if (Vector3.Distance(m_Rigidbody.position, m_TargetPos) < 0.5f)
        {
            SetNewTarget();
            TryJump();
        }
    }

    private void TryJump()
    {
        if (m_IsGrounded && Random.value < m_JumpChance)
        {
            m_Rigidbody.AddForce(Vector3.up * m_JumpForce, ForceMode.Impulse);
            m_IsGrounded = false;
        }
    }
}
