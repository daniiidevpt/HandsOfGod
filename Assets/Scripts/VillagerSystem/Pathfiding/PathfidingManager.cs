using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HOG.Pathfiding
{
    public class PathfidingManager : Singleton<PathfidingManager>
    {
        private Queue<PathRequest> m_PathRequestQueue = new Queue<PathRequest>();
        private PathRequest m_CurrentPathRequest;
        private bool m_IsProcessingPath;

        public void RequestPath(Vector3 startPos, Vector3 targetPos, System.Action<Vector3[], bool> callback)
        {
            PathRequest newRequest = new PathRequest(startPos, targetPos, callback);
            m_PathRequestQueue.Enqueue(newRequest);
            ProcessNextPath();
        }

        private void ProcessNextPath()
        {
            if (!m_IsProcessingPath && m_PathRequestQueue.Count > 0)
            {
                m_CurrentPathRequest = m_PathRequestQueue.Dequeue();
                m_IsProcessingPath = true;
                StartCoroutine(CalculatePath(m_CurrentPathRequest));
            }
        }

        private IEnumerator CalculatePath(PathRequest request)
        {
            yield return null;

            NavMeshPath navMeshPath = new NavMeshPath();
            bool pathSuccess = NavMesh.CalculatePath(request.m_StartPosition, request.m_TargetPosition, NavMesh.AllAreas, navMeshPath);

            if (pathSuccess && navMeshPath.corners.Length > 1)
            {
                // Path smoothing (ensures smooth turns)
                Vector3[] smoothPath = SmoothPath(navMeshPath.corners);
                request.m_Callback(smoothPath, true);
            }
            else
            {
                request.m_Callback(null, false);
            }

            m_IsProcessingPath = false;
            ProcessNextPath();
        }

        private Vector3[] SmoothPath(Vector3[] corners)
        {
            List<Vector3> smoothedPath = new List<Vector3>();

            for (int i = 0; i < corners.Length; i++)
            {
                if (i == 0 || i == corners.Length - 1 || Vector3.Distance(corners[i - 1], corners[i + 1]) > 1.5f)
                {
                    smoothedPath.Add(corners[i]);
                }
            }

            return smoothedPath.ToArray();
        }
    }
}
