using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.HandPoser;
using HurricaneVR.Framework.Shared;
using System.Collections.Generic;
using UnityEngine;

namespace HOG.Powers
{
    public class PowersTrigger : MonoBehaviour
    {
        [Header("Trigger Settings")]
        [SerializeField] protected LayerMask m_Layer;

        [Header("Grab Settings")]
        [SerializeField] private HVRGrabTrigger m_GrabTrigger;

        [Header("Powers Settings")]
        [SerializeField] private PowersType m_PowerType;
        [SerializeField] private GameObject m_PowerPrefab;
        [SerializeField] private bool m_LeftPowerUsed = false;
        [SerializeField] private bool m_RightPowerUsed = false;

        private readonly HashSet<Collider> m_LeftHandColliders = new HashSet<Collider>();
        private readonly HashSet<Collider> m_RightHandColliders = new HashSet<Collider>();

        private void OnTriggerStay(Collider other)
        {
            if (((1 << other.gameObject.layer) & m_Layer) == 0) return;

            HVRHandGrabber hand = other.GetComponentInParent<HVRHandGrabber>();
            if (hand == null) return;

            if (hand.HandSide == HVRHandSide.Left)
            {
                m_LeftHandColliders.Add(other);

                if (!m_LeftPowerUsed && HVRInputManager.Instance.LeftController.GripButtonState.JustActivated)
                {
                    Debug.Log("Left Hand Power");
                    m_LeftPowerUsed = true;

                    GameObject powerInst = Instantiate(m_PowerPrefab, other.transform.position, Quaternion.identity);

                    HVRGrabbable grabbable = powerInst.GetComponent<HVRGrabbable>();
                    HVRPosableGrabPoint grabPoint = grabbable.GrabPointsMeta[0];

                    Grab(hand, grabbable, grabPoint);
                }
            }
            else if (hand.HandSide == HVRHandSide.Right)
            {
                m_RightHandColliders.Add(other);

                if (!m_RightPowerUsed && HVRInputManager.Instance.RightController.GripButtonState.JustActivated)
                {
                    Debug.Log("Right Hand Power");
                    m_RightPowerUsed = true;

                    GameObject powerInst = Instantiate(m_PowerPrefab);

                    HVRGrabbable grabbable = powerInst.GetComponent<HVRGrabbable>();
                    HVRPosableGrabPoint grabPoint = grabbable.GrabPointsMeta[0];

                    Grab(hand, grabbable, grabPoint);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (((1 << other.gameObject.layer) & m_Layer) == 0) return;

            HVRHandGrabber hand = other.GetComponentInParent<HVRHandGrabber>();
            if (hand == null) return;

            if (hand.HandSide == HVRHandSide.Left)
            {
                m_LeftHandColliders.Remove(other);
                if (m_LeftHandColliders.Count == 0)
                {
                    m_LeftPowerUsed = false;
                }
            }
            else if (hand.HandSide == HVRHandSide.Right)
            {
                m_RightHandColliders.Remove(other);
                if (m_RightHandColliders.Count == 0)
                {
                    m_RightPowerUsed = false;
                }
            }
        }

        private void Grab(HVRHandGrabber grabber, HVRGrabbable grabbable, HVRPosableGrabPoint grabPoint)
        {
            if (grabbable && grabber)
            {
                if (m_GrabTrigger == HVRGrabTrigger.ManualRelease && grabber.GrabbedTarget == grabbable)
                {
                    grabber.ForceRelease();
                    return;
                }

                //grabber needs to have it's release sequence completed if it's holding something
                if (grabber.IsGrabbing)
                    grabber.ForceRelease();
                grabber.Grab(grabbable, m_GrabTrigger, grabPoint);
            }
        }
    }
}
