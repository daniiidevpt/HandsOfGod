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

        protected override void Awake()
        {
            base.Awake();
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

            string timeOfDay = GetTimeOfDay(m_CurrentTime);
            //Debug.Log($"Time: {Mathf.Floor(m_CurrentTime)}:00 - {timeOfDay}");
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
