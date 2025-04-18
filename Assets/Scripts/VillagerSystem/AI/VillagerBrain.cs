﻿using HOG.Grid;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace HOG.Villager
{
    public class VillagerBrain : MonoBehaviour
    {
        [Header("Locomotion Settings")]
        [SerializeField] private List<Transform> m_Waypoints = new List<Transform>();
        public List<Transform> Waypoints => m_Waypoints;

        [Header("Sensor Settings")]
        [SerializeField] private List<string> m_Tags = new List<string>();
        [SerializeField] private float m_SensorRadius = 10f;
        public List<string> Tags => m_Tags;
        public float SensorRadius => m_SensorRadius;

        [Header("Tools Settings")]
        [SerializeField] private AnimationClip m_VillagerSwingAnim;
        [SerializeField] private GameObject m_Hammer;
        [SerializeField] private GameObject m_Shovel;
        [SerializeField] private GameObject m_Sword;

        public AnimationClip VillagerSwingAnim => m_VillagerSwingAnim;
        public GameObject Hammer => m_Hammer;
        public GameObject Shovel => m_Shovel;
        public GameObject Sword => m_Sword;

        private Rigidbody m_Rigidbody;
        private HVRGrabbable m_Grabbable;

        private StateMachine m_StateMachine;
        private UtilitySelector m_UtilitySelector;

        #region States
        public IdleState IdleState { get; private set; }
        public PatrolState PatrolState { get; private set; }
        public  CollectWoodState CollectWoodState { get; private set; }
        public CollectRockState CollectRockState { get; private set; }
        public PanicState PanicState { get; private set; }
        #endregion

        private VillagerSensor m_Sensor;
        private VillagerLocomotion m_Locomotion;

        public Rigidbody GetRigidbody() => m_Rigidbody;
        public StateMachine GetStateMachine() => m_StateMachine;
        public VillagerSensor GetSensor() => m_Sensor;
        public VillagerLocomotion GetLocomotion() => m_Locomotion;

        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Grabbable = GetComponent<HVRGrabbable>();

            m_Sensor = GetComponentInChildren<VillagerSensor>();
            m_Locomotion = GetComponentInChildren<VillagerLocomotion>();
            m_Sensor.Initialize();
            m_Locomotion.Initialize();
        }

        private void Start()
        {
            IdleState = new IdleState(this);
            PatrolState = new PatrolState(this);
            CollectWoodState = new CollectWoodState(this);
            CollectRockState = new CollectRockState(this);
            PanicState = new PanicState(this);

            m_UtilitySelector = new UtilitySelector();
            m_UtilitySelector.RegisterState(CollectWoodState);
            m_UtilitySelector.RegisterState(CollectRockState);

            m_StateMachine = new StateMachine();
            m_StateMachine.ChangeState(PatrolState);

            m_StateMachine.RegisterEventTransition("Grabbed", IdleState);
            //m_StateMachine.RegisterEventTransition("Released", PatrolState);
            m_StateMachine.RegisterEventTransition("LowOnWood", CollectWoodState);
            m_StateMachine.RegisterEventTransition("LowOnRock", CollectRockState);
            m_StateMachine.RegisterEventTransition("Panic", PanicState);

            //m_Locomotion.SetDestination(GridManager.Instance.GetNodeFromWorld(new Vector3(8, 0, 8)));
            //m_Locomotion.OnDestinationReached += () =>
            //{
            //    Debug.Log("Arrived at place!");
            //};
        }

        private void Update()
        {
            m_StateMachine.Update();

            if (Input.GetKeyDown(KeyCode.W))
            {
                m_StateMachine.TriggerEvent("LowOnWood");
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                m_StateMachine.TriggerEvent("LowOnRock");
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                m_StateMachine.TriggerEvent("Panic");
            }
        }

        private void OnEnable()
        {
            m_Grabbable.Grabbed.AddListener(OnGrabbed);
            m_Grabbable.Released.AddListener(OnReleased);

            m_Grabbable.HoverEnter.AddListener(OnHoverEnter);
            m_Grabbable.HoverExit.AddListener(OnHoverExit);
        }

        private void OnDisable()
        {
            m_Grabbable.Grabbed.RemoveListener(OnGrabbed);
            m_Grabbable.Released.RemoveListener(OnReleased);

            m_Grabbable.HoverEnter.RemoveListener(OnHoverEnter);
            m_Grabbable.HoverExit.RemoveListener(OnHoverExit);
        }

        public void OnGrabbed(HVRGrabberBase grabberBase, HVRGrabbable grabbable)
        {
            m_Locomotion.PauseMovement();
            m_StateMachine.TriggerEvent("Grabbed");

            RigidbodyConstraints unfreeze = RigidbodyConstraints.None;
            m_Rigidbody.constraints = unfreeze;
        }

        public void OnReleased(HVRGrabberBase grabberBase, HVRGrabbable grabbable)
        {
            m_Locomotion.ResumeMovement();

            RigidbodyConstraints freezeX = RigidbodyConstraints.FreezeRotationX;
            RigidbodyConstraints freezeZ = RigidbodyConstraints.FreezeRotationZ;
            m_Rigidbody.constraints = freezeX | freezeZ;

            Vector3 dropPosition = new Vector3(transform.position.x, 0f, transform.position.z);
            var bestState = m_UtilitySelector.GetBestState(dropPosition);

            if (bestState != null)
            {
                m_StateMachine.ChangeState(bestState);
            }
            else
            {
                m_StateMachine.ChangeState(PatrolState);
            }
        }

        public void OnHoverEnter(HVRGrabberBase grabberBase, HVRGrabbable grabbable)
        {
            if (!m_Grabbable.IsHandGrabbed)
            {
                m_Locomotion.PauseMovement();
            }
        }

        public void OnHoverExit(HVRGrabberBase grabberBase, HVRGrabbable grabbable)
        {
            if (!m_Grabbable.IsHandGrabbed)
            {
                m_Locomotion.ResumeMovement();
            }
        }

        public IEnumerator PlayVillagerSwing(GameObject toolObject, AnimationClip clip, float duration)
        {
            toolObject.SetActive(true);

            var animator = toolObject.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError($"Tool '{toolObject.name}' does not have an Animator component.");
                yield break;
            }

            var graph = PlayableGraph.Create();
            var output = AnimationPlayableOutput.Create(graph, "ToolSwing", animator);
            var clipPlayable = AnimationClipPlayable.Create(graph, clip);

            clipPlayable.SetDuration(duration);
            clipPlayable.SetTime(0);
            clipPlayable.SetSpeed(1);

            output.SetSourcePlayable(clipPlayable);
            graph.Play();

            yield return new WaitForSeconds(duration);

            graph.Stop();
            graph.Destroy();

            toolObject.SetActive(false);
        }
    }
}
