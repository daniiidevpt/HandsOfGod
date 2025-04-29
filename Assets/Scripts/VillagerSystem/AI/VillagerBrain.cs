using HOG.Building;
using HOG.Grid;
using HOG.Resources;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace HOG.Villager
{
    public class VillagerBrain : MonoBehaviour
    {
        [Header("Settings SO")]
        [SerializeField] private VillagerSettingsSO m_Settings;
        public VillagerSettingsSO Settings => m_Settings;

        [Header("Villager Settings")]
        [SerializeField] private List<Transform> m_Waypoints = new List<Transform>();
        private bool m_IsGrounded = false;
        private bool m_IsBusy = false;

        public List<Transform> Waypoints => m_Waypoints;
        public List<string> Tags => m_Settings.Tags;
        public float SensorRadius => m_Settings.SensorRadius;
        public bool IsGrounded => m_IsGrounded;
        public bool IsBusy { get => m_IsBusy; set => m_IsBusy = value; }

        [Header("Evaluation Settings")]
        [SerializeField]private float m_EvalutationMaxRange = 3f;
        private float m_EvaluationInterval = 0f;
        private float m_EvaluationTimer = 0f;

        [Header("Tools Settings")]
        [SerializeField] private AnimationClip m_VillagerSwingAnim;
        [SerializeField] private GameObject m_Hammer;
        [SerializeField] private GameObject m_Shovel;
        [SerializeField] private GameObject m_Sword;

        [Header("Villager Menu")]
        [SerializeField] private TextMeshProUGUI m_VillagerTypeText;

        private Rigidbody m_Rigidbody;
        private HVRGrabbable m_Grabbable;
        private Animator m_Animator;

        private StateMachine m_StateMachine;

        #region States
        public IdleState IdleState { get; private set; }
        public PatrolState PatrolState { get; private set; }
        public  CollectWoodState CollectWoodState { get; private set; }
        public CollectRockState CollectRockState { get; private set; }
        public BuildingState BuildingState { get; private set; }
        public PanicState PanicState { get; private set; }
        public TalkState TalkState { get; private set; }
        #endregion

        private VillagerSensor m_Sensor;
        private VillagerLocomotion m_Locomotion;

        public Rigidbody GetRigidbody() => m_Rigidbody;
        public StateMachine GetStateMachine() => m_StateMachine;
        public VillagerSensor GetSensor() => m_Sensor;
        public VillagerLocomotion GetLocomotion() => m_Locomotion;

        //debug
        private TextMeshProUGUI m_DebugStateText;

        private enum VillagerPhysicsState
        {
            Idle,
            Grabbed,
            Thrown,
            Landed
        }

        private VillagerPhysicsState m_PhysicsState = VillagerPhysicsState.Idle;

        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Grabbable = GetComponent<HVRGrabbable>();
            m_Animator = GetComponentInChildren<Animator>();

            m_Sensor = GetComponentInChildren<VillagerSensor>();
            m_Locomotion = GetComponentInChildren<VillagerLocomotion>();
            m_Sensor.Initialize();
            m_Locomotion.Initialize();

            InitializeUI();

            m_EvaluationInterval = Random.Range(1f, m_EvalutationMaxRange);
            m_EvaluationTimer = 0f;

            m_DebugStateText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start()
        {
            IdleState = new IdleState(this, "Idle");
            PatrolState = new PatrolState(this, "Patrol");
            CollectWoodState = new CollectWoodState(this, "CollectWoodState");
            CollectRockState = new CollectRockState(this, "CollectRockState");
            BuildingState = new BuildingState(this, "BuildingState");
            PanicState = new PanicState(this, "PanicState");
            TalkState = new TalkState(this, "TalkState");

            m_StateMachine = new StateMachine();
            m_StateMachine.ChangeState(PatrolState);

            m_StateMachine.RegisterEventTransition("Grabbed", IdleState);
            m_StateMachine.RegisterEventTransition("LowOnWood", CollectWoodState);
            m_StateMachine.RegisterEventTransition("LowOnRock", CollectRockState);
            m_StateMachine.RegisterEventTransition("Panic", PanicState);
        }

        private void Update()
        {
            m_StateMachine.Update();
            m_DebugStateText.text = m_StateMachine.GetCurrentStateName();

            m_IsGrounded = Physics.CheckSphere(transform.position, m_Settings.DetectionRadius, m_Settings.DetectionLayer);

            if (m_PhysicsState == VillagerPhysicsState.Thrown && m_IsGrounded)
            {
                HandleLanding();
                return;
            }

            if (m_StateMachine.CurrentState == PatrolState)
            {
                m_EvaluationTimer += Time.deltaTime;
                if (m_EvaluationTimer >= m_EvaluationInterval)
                {
                    m_EvaluationTimer = 0f;
                    m_EvaluationInterval = Random.Range(1f, m_EvalutationMaxRange);


                    float randomChance = Random.value;
                    if (randomChance <= 0.1f)
                    {
                        TryRequestTalk();
                    }
                    else
                    {
                        Evaluate();
                    }
                }
            }

            // Debug events
            if (Input.GetKeyDown(KeyCode.W)) m_StateMachine.TriggerEvent("LowOnWood");
            if (Input.GetKeyDown(KeyCode.R)) m_StateMachine.TriggerEvent("LowOnRock");
            if (Input.GetKeyDown(KeyCode.P)) m_StateMachine.TriggerEvent("Panic");
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

        #region VR Interactions

        public void OnGrabbed(HVRGrabberBase grabberBase, HVRGrabbable grabbable)
        {
            m_PhysicsState = VillagerPhysicsState.Grabbed;

            m_Locomotion.PauseMovement();
            m_StateMachine.TriggerEvent("Grabbed");

            RigidbodyConstraints unfreeze = RigidbodyConstraints.None;
            m_Rigidbody.constraints = unfreeze;
        }

        public void OnReleased(HVRGrabberBase grabberBase, HVRGrabbable grabbable)
        {
            m_PhysicsState = VillagerPhysicsState.Thrown;
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

        private void HandleLanding()
        {
            m_PhysicsState = VillagerPhysicsState.Landed;

            var euler = transform.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, euler.y, 0f);

            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            m_Locomotion.ResumeMovement();

            m_EvaluationTimer = 0f;

            Evaluate();
        }

        #endregion

        #region Tools Animations

        public IEnumerator PlayHammerSwing(float duration) => PlayVillagerSwing(m_Hammer, m_VillagerSwingAnim, duration);

        public IEnumerator PlayShovelSwing(float duration) => PlayVillagerSwing(m_Shovel, m_VillagerSwingAnim, duration);

        public IEnumerator PlaySwordSwing(float duration) => PlayVillagerSwing(m_Sword, m_VillagerSwingAnim, duration);

        private IEnumerator PlayVillagerSwing(GameObject toolObject, AnimationClip clip, float duration)
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

        public void DisableTools()
        {
            m_Hammer.SetActive(false);
            m_Shovel.SetActive(false);
            m_Sword.SetActive(false);
        }

        #endregion

        #region Villagers Animations

        public void PlayVillagerTalk() => m_Animator.SetBool("IsTalking", true);
        public void StopVillagerTalk() => m_Animator.SetBool("IsTalking", false);

        #endregion

        #region Villager UI

        private void InitializeUI()
        {
            m_VillagerTypeText.text = $"Type: {m_Settings.Type}";
        }

        #endregion

        private void Evaluate()
        {
            if (m_Settings.Tags == null || m_Settings.Tags.Count == 0)
            {
                Debug.LogWarning("No tags assigned to villager.");
                return;
            }

            var target = m_Sensor.GetClosestTarget(m_Settings.Tags);
            if (target == null)
            {
                m_StateMachine.ChangeState(PatrolState);
                return;
            }

            StateContext ctx = new StateContext { Target = target };

            if (target.TryGetComponent(out Wood wood) && !wood.IsCollected)
            {
                ctx.TargetResource = wood;
                CollectWoodState.SetContext(ctx);
                m_StateMachine.ChangeState(CollectWoodState);
                return;
            }

            if (target.TryGetComponent(out Rock rock) && !rock.IsCollected)
            {
                ctx.TargetResource = rock;
                CollectRockState.SetContext(ctx);
                m_StateMachine.ChangeState(CollectRockState);
                return;
            }

            if (target.TryGetComponent(out HouseBuilder builder) && builder.IsBuildable)
            {
                builder.VillagerInBuilding = this;
                ctx.TargetBuilding = builder;
                BuildingState.SetContext(ctx);
                m_StateMachine.ChangeState(BuildingState);
                return;
            }
        }

        public void TryRequestTalk()
        {
            var partner = VillagersManager.Instance.FindClosestAvailableVillager(transform.position, this);
            if (partner == null) return;

            Vector3 meetingPoint = (transform.position + partner.transform.position) / 2f;

            TalkState.SetContext(partner, meetingPoint);
            partner.TalkState.SetContext(this, meetingPoint);

            m_StateMachine.ChangeState(TalkState);
            partner.GetStateMachine().ChangeState(partner.TalkState);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = m_IsGrounded ? Color.cyan : Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.1f);
        }
    }
}
