using HOG.Resources;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

namespace HOG.Villager
{
    public class VillagerBrain : MonoBehaviour
    {
        [Header("Sensor Settings")]
        [SerializeField] private List<string> m_Tags = new List<string>();
        [SerializeField] private float m_SensorRadius = 10f;

        public List<string> Tags => m_Tags;

        public float SensorRadius => m_SensorRadius;

        private Rigidbody m_Rigidbody;
        private VillagerSensor m_Sensor;
        private HVRGrabbable m_Grabbable;
        private BehaviorGraphAgent m_Graph;

        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Sensor = GetComponentInChildren<VillagerSensor>();
            m_Grabbable = GetComponent<HVRGrabbable>();
            m_Graph = GetComponent<BehaviorGraphAgent>();
        }

        private void OnEnable()
        {
            m_Grabbable.Grabbed.AddListener(SetIsGrabbedTrue);
            m_Grabbable.Released.AddListener(SetIsGrabbedFalse);
        }

        private void OnDisable()
        {
            m_Grabbable.Grabbed.RemoveListener(SetIsGrabbedTrue);
            m_Grabbable.Released.RemoveListener(SetIsGrabbedFalse);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                m_Graph.SetVariableValue("IsGrabbed", true);
            }

            if (Input.GetKeyUp(KeyCode.V))
            {
                m_Graph.SetVariableValue("IsGrabbed", false);
            }

        }

        public void SetIsGrabbedTrue(HVRGrabberBase grabberBase, HVRGrabbable grabbable)
        {
            m_Graph.SetVariableValue("IsGrabbed", true);
            RigidbodyConstraints unfreeze = RigidbodyConstraints.None;
            m_Rigidbody.constraints = unfreeze;
        }

        public void SetIsGrabbedFalse(HVRGrabberBase grabberBase, HVRGrabbable grabbable)
        {
            m_Graph.SetVariableValue("IsGrabbed", false);
            RigidbodyConstraints freezeX = RigidbodyConstraints.FreezeRotationX;
            RigidbodyConstraints freezeZ = RigidbodyConstraints.FreezeRotationZ;
            m_Rigidbody.constraints = freezeX | freezeZ;
        }
    }
}
