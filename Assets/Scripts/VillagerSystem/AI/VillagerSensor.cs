using System;
using System.Collections.Generic;
using UnityEngine;

namespace HOG.Villager
{
    [RequireComponent(typeof(SphereCollider))]
    public class VillagerSensor : MonoBehaviour
    {
        private VillagerBrain m_Brain;
        private SphereCollider m_SensorCollider;

        readonly List<Transform> m_DetectedObjects = new List<Transform>();

        private void Start()
        {
            m_Brain = GetComponentInParent<VillagerBrain>();

            m_SensorCollider = GetComponent<SphereCollider>();
            m_SensorCollider.isTrigger = true;
            m_SensorCollider.radius = m_Brain.SensorRadius;

            float worldRadius = m_Brain.SensorRadius * transform.lossyScale.x;
            Collider[] colliders = Physics.OverlapSphere(transform.position, worldRadius);

            foreach (var c in colliders)
            {
                ProcessTrigger(c, transform => m_DetectedObjects.Add(transform));
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            float worldRadius = m_Brain.SensorRadius * transform.lossyScale.x;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, worldRadius);

            Gizmos.color = Color.red;
            foreach (Transform obj in m_DetectedObjects)
            {
                if (obj != null)
                {
                    Gizmos.DrawSphere(obj.position, 0.2f);
                }
            }
        }

        //private void Update()
        //{
        //    Debug.Log($"Detected Objects Count: {m_DetectedObjects.Count}");
        //}

        private void OnTriggerEnter(Collider other)
        {
            ProcessTrigger(other, transform =>
            {
                if (!m_DetectedObjects.Contains(transform))
                {
                    m_DetectedObjects.Add(transform);
                    //Debug.Log($"Added {transform.name} to detected list");
                }
            });
        }

        private void OnTriggerExit(Collider other)
        {
            ProcessTrigger(other, transform =>
            {
                if (m_DetectedObjects.Contains(transform))
                {
                    m_DetectedObjects.Remove(transform);
                    //Debug.Log($"Removed {transform.name} from detected list");
                }
            });
        }

        private void ProcessTrigger(Collider other, Action<Transform> action)
        {
            if (other.CompareTag("Untagged")) return;

            foreach (string t in m_Brain.Tags)
            {
                if (other.CompareTag(t))
                {
                    action(other.transform);
                }
            }
        }

        public Transform GetClosestTarget(string tag)
        {
            if (m_DetectedObjects.Count == 0) return null;

            Transform closestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;

            m_DetectedObjects.RemoveAll(obj => obj == null || !obj.CompareTag(tag));

            foreach (Transform potentialTarget in m_DetectedObjects)
            {
                if (potentialTarget.CompareTag(tag))
                {
                    Vector3 directionToTarget = potentialTarget.position - currentPosition;
                    float dSqrToTarget = directionToTarget.sqrMagnitude;

                    if (dSqrToTarget < closestDistanceSqr)
                    {
                        closestDistanceSqr = dSqrToTarget;
                        closestTarget = potentialTarget;
                    }
                }
            }

            return closestTarget;
        }
    }
}
