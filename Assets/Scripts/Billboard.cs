using UnityEngine;

namespace HOG
{
    public class Billboard : MonoBehaviour
    {
        private Transform m_Camera;

        private void Start()
        {
            m_Camera = Camera.main.transform;
            if (Camera.main == null)
            {
                Debug.LogError("MainCamera not found");
            }
        }

        private void Update()
        {
            Vector3 direction = transform.position - m_Camera.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
        }
    }
}
