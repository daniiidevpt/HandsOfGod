using HOG.Resources;
using System.Collections;
using UnityEngine;

namespace HOG.Building
{
    public class HouseBuilder : MonoBehaviour
    {
        [Header("Building Settings")]
        [SerializeField] private BuildingStage m_CurrentStage = BuildingStage.Stage1;
        [SerializeField] private float m_StageDurationMinutes = 1f;
        [SerializeField] private GameObject[] m_StageObjects;
        private int m_CurrentStageIndex = 0;

        [Header("Building Cost")]
        [SerializeField] private int m_WoodAmount = 5;
        [SerializeField] private int m_RockAmount = 5;

        private void Start()
        {
            for (int i = 0; i < m_StageObjects.Length; i++)
            {
                m_StageObjects[i].SetActive(i == 0);
            }

            StartCoroutine(BuildHouse());
        }

        public void StartBuildingHouse()
        {
            if (ResourceManager.Instance.GetResourceAmount(ResourceType.Wood) >= m_WoodAmount && ResourceManager.Instance.GetResourceAmount(ResourceType.Rock) >= m_RockAmount)
            {
                StartCoroutine(BuildHouse());
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
