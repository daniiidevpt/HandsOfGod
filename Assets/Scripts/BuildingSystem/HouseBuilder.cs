using HOG.Resources;
using HOG.Villager;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HOG.Building
{
    public class HouseBuilder : MonoBehaviour
    {
        [Header("Building Settings")]
        [SerializeField] private BuildingStage m_CurrentStage = BuildingStage.Stage1;
        [SerializeField] private float m_StageDurationMinutes = 1f;
        [SerializeField] private GameObject[] m_StageObjects;
        private int m_CurrentStageIndex = 0;
        private bool m_IsFinished = false;

        [Header("Building Cost")]
        [SerializeField] private int m_WoodAmount = 5;
        [SerializeField] private int m_RockAmount = 5;

        [Header("UI Settings")]
        [SerializeField] private Button m_BuildButton;
        [SerializeField] private TextMeshProUGUI m_WoodCostText;
        [SerializeField] private TextMeshProUGUI m_RockCostText;

        private VillagerBrain m_VillagerInBuilding;

        private void Start()
        {
            for (int i = 0; i < m_StageObjects.Length; i++)
            {
                m_StageObjects[i].SetActive(i == 0);
            }

            m_WoodCostText.text = $"Wood: {m_WoodAmount}";
            m_RockCostText.text = $"Wood: {m_RockAmount}";

            Invoke("StartBuildingHouse", 5f);
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
                VillagersManager.Instance.UnassignVillagerFromBuilding(m_VillagerInBuilding);
                m_IsFinished = true;
            }
        }

        public void StartBuildingHouse()
        {
            if (ResourceManager.Instance.GetResourceAmount(ResourceType.Wood) >= m_WoodAmount && ResourceManager.Instance.GetResourceAmount(ResourceType.Rock) >= m_RockAmount)
            {
                StartCoroutine(BuildHouse());
                m_VillagerInBuilding = VillagersManager.Instance.AssignVillagerToBuilding(this.transform.position);
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
    }
}
