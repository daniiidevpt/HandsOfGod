using Unity.Behavior;
using UnityEngine;

namespace HOG
{
    public class TimeManager : Singleton<TimeManager>
    {
        [Header("Time Settings")]
        [SerializeField] private int m_DayDurationInMinutes = 5;
        private float m_CurrentTime = 6f; // Start the day at 6:00 AM why not :/
        private float m_TimeSpeed;

        public float CurrentTime => m_CurrentTime;

        private BehaviorGraphAgent m_Graph;

        protected override void Awake()
        {
            base.Awake();

            m_Graph = GetComponentInParent<BehaviorGraphAgent>();
        }

        private void Start()
        {
            m_TimeSpeed = 24f / (m_DayDurationInMinutes * 60f);
        }

        private void Update()
        {
            SimulateTime();
        }

        private void SimulateTime()
        {
            m_CurrentTime += Time.deltaTime * m_TimeSpeed;

            if (m_CurrentTime >= 24f)
            {
                m_CurrentTime = 0f;
            }

            SetGlobalTime(m_CurrentTime);

            string timeOfDay = GetTimeOfDay(m_CurrentTime);
            //Debug.Log($"Time: {Mathf.Floor(m_CurrentTime)}:00 - {timeOfDay}");
        }

        private void SetGlobalTime(float hour)
        {
            if (hour >= 6 && hour < 12)
            {
                m_Graph.SetVariableValue("GLOBAL_DayPhase", DayPhase.Morning);

                if (m_Graph.GetVariable("GLOBAL_DayPhase", out BlackboardVariable dayPhase))
                    Debug.Log($"GLOBAL DayPhase: {dayPhase.ObjectValue}");
            }
            else if (hour >= 12 && hour < 18)
            {
                m_Graph.SetVariableValue("GLOBAL_DayPhase", DayPhase.Afternoon);

                if (m_Graph.GetVariable("GLOBAL_DayPhase", out BlackboardVariable dayPhase))
                    Debug.Log($"GLOBAL DayPhase: {dayPhase.ObjectValue}");
            }
            else if (hour >= 18 && hour < 22)
            {
                m_Graph.SetVariableValue("GLOBAL_DayPhase", DayPhase.Evening);

                if (m_Graph.GetVariable("GLOBAL_DayPhase", out BlackboardVariable dayPhase))
                    Debug.Log($"GLOBAL DayPhase: {dayPhase.ObjectValue}");
            }
            else 
            {
                m_Graph.SetVariableValue("GLOBAL_DayPhase", DayPhase.Night);

                if (m_Graph.GetVariable("GLOBAL_DayPhase", out BlackboardVariable dayPhase))
                    Debug.Log($"GLOBAL DayPhase: {dayPhase.ObjectValue}");
            }
        }

        public string GetTimeOfDay(float hour)
        {
            if (hour >= 6 && hour < 12)
                return "Morning";
            else if (hour >= 12 && hour < 18)
                return "Afternoon";
            else if (hour >= 18 && hour < 22)
                return "Evening";
            else
                return "Night";
        }
    }
}
