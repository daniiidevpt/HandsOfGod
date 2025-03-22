using UnityEngine;
using System;

namespace HOG.Pathfiding
{
    public struct PathRequest
    {
        public Vector3 m_StartPosition;
        public Vector3 m_TargetPosition;
        public Action<Vector3[], bool> m_Callback;

        public PathRequest(Vector3 start, Vector3 target, Action<Vector3[], bool> callback)
        {
            m_StartPosition = start;
            m_TargetPosition = target;
            m_Callback = callback;
        }
    }
}
