using System.Collections.Generic;
using UnityEngine;

namespace HOG.Grid
{
    public class PathDebugVisualizer : MonoBehaviour
    {
        private Dictionary<string, List<Vector3>> m_Paths = new();
        public Color m_PathColor = Color.yellow;
        public Color m_StartColor = Color.green;
        public Color m_EndColor = Color.red;
        public float m_SphereSize = 0.15f;

        public void SetPath(string id, List<Vector3> path)
        {
            if (path == null || path.Count == 0) return;

            m_Paths[id] = path;
        }

        public void ClearPath(string id)
        {
            if (m_Paths.ContainsKey(id))
                m_Paths.Remove(id);
        }

        public void ClearAll()
        {
            m_Paths.Clear();
        }

        private void OnDrawGizmos()
        {
            if (m_Paths == null || m_Paths.Count == 0)
                return;

            foreach (var keyValuePair in m_Paths)
            {
                var path = keyValuePair.Value;
                if (path == null || path.Count == 0)
                    continue;

                for (int i = 0; i < path.Count; i++)
                {
                    Vector3 pos = path[i] + Vector3.up * 0.1f;

                    if (i == 0)
                        Gizmos.color = m_StartColor;
                    else if (i == path.Count - 1)
                        Gizmos.color = m_EndColor;
                    else
                        Gizmos.color = m_PathColor;

                    Gizmos.DrawSphere(pos, m_SphereSize);

                    if (i < path.Count - 1)
                    {
                        Gizmos.DrawLine(pos, path[i + 1] + Vector3.up * 0.1f);
                    }
                }
            }
        }
    }
}
