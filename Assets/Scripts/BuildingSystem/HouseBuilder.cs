using HOG.Resources;
using HOG.Villager;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HOG.Building
{
    public class HouseBuilder : MonoBehaviour, IBurnable
    {
        [Header("Building Settings")]
        [SerializeField] private BuildingStage m_CurrentStage = BuildingStage.Stage1;
        [SerializeField] private float m_StageDurationMinutes = 1f;
        [SerializeField] private Transform m_BuildingPosition = null;
        [SerializeField] private GameObject[] m_StageObjects;
        private int m_CurrentStageIndex = 0;
        private bool m_IsFinished = false;
        private bool m_IsBuildable = false;
        private Coroutine m_BuildRoutine;

        [Header("Fire VFX")]
        [SerializeField] private GameObject m_FireVFX;
        [SerializeField] private int m_BurnTime = 3;
        [SerializeField] private GameObject m_DestroyedHouse;
        private bool m_IsBurning;

        public Transform BuildingPosition => m_BuildingPosition;
        public bool IsFinsihed => m_IsFinished;
        public bool IsBuildable => m_IsBuildable;
        public bool IsBurning => m_IsBurning;

        [Header("Building Cost")]
        [SerializeField] private int m_WoodAmount = 5;
        [SerializeField] private int m_RockAmount = 5;

        [Header("UI Settings")]
        [SerializeField] private GameObject m_Canvas;
        [SerializeField] private Button m_BuildButton;
        [SerializeField] private TextMeshProUGUI m_WoodCostText;
        [SerializeField] private TextMeshProUGUI m_RockCostText;

        private VillagerBrain m_VillagerInBuilding;
        public VillagerBrain VillagerInBuilding { get => m_VillagerInBuilding; set => m_VillagerInBuilding = value; }

        private void Start()
        {
            for (int i = 0; i < m_StageObjects.Length; i++)
            {
                m_StageObjects[i].SetActive(i == 0);
            }

            m_WoodCostText.text = $"Wood: {m_WoodAmount}";
            m_RockCostText.text = $"Wood: {m_RockAmount}";
        }

        private void OnEnable()
        {
            m_BuildButton.onClick.AddListener(StartBuildingHouse);
        }

        private void OnDisable()
        {
            m_BuildButton.onClick.RemoveListener(StartBuildingHouse);
        }

        private void Update()
        {
            if (!m_IsFinished && m_CurrentStage == BuildingStage.Stage4)
            {
                if (m_VillagerInBuilding != null)
                {
                    m_VillagerInBuilding.GetStateMachine().ChangeState(m_VillagerInBuilding.PatrolState);
                    m_VillagerInBuilding = null;
                }

                m_IsFinished = true;
                m_IsBuildable = false;
            }
        }

        public void StartBuildingHouse()
        {
            if (ResourceManager.Instance.GetResourceAmount(ResourceType.Wood) >= m_WoodAmount && ResourceManager.Instance.GetResourceAmount(ResourceType.Rock) >= m_RockAmount)
            {
                m_Canvas.SetActive(false);
                m_IsBuildable = true;

                m_BuildRoutine = StartCoroutine(BuildHouse());
            }
            else
            {
                Debug.Log("Not enough resources");
            }
        }

        private IEnumerator BuildHouse()
        {
            float timeToWait = m_StageDurationMinutes * 60f;

            while (m_CurrentStageIndex < m_StageObjects.Length - 1)
            {
                yield return new WaitForSeconds(timeToWait);

                if (m_CurrentStageIndex < m_StageObjects.Length - 1)
                {
                    m_StageObjects[m_CurrentStageIndex].SetActive(false);

                    m_CurrentStageIndex++;
                    m_CurrentStage = (BuildingStage)m_CurrentStageIndex;

                    m_StageObjects[m_CurrentStageIndex].SetActive(true);
                }
            }
        }

        public void Ignite()
        {
            if (m_IsBurning) return;

            if (m_BuildRoutine != null)
            {
                StopCoroutine(m_BuildRoutine);
            }

            m_IsBurning = true;
            GameObject fire = Instantiate(m_FireVFX, transform.position + Vector3.up, Quaternion.identity);
            Destroy(fire, m_BurnTime);

            StartCoroutine(BurnUp());
        }

        private IEnumerator BurnUp()
        {
            yield return new WaitForSeconds(m_BurnTime);
            m_StageObjects[m_CurrentStageIndex].SetActive(false);
            m_DestroyedHouse.SetActive(true);
        }
    }
}
