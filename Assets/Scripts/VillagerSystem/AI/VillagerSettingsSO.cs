using System.Collections.Generic;
using UnityEngine;

namespace HOG.Villager
{
    [CreateAssetMenu(fileName = "VillagerSettings", menuName = "ScriptableObjects/Villager/Settings", order = 1)]
    public class VillagerSettingsSO : ScriptableObject
    {
        public enum VillagerType
        {
            Kid,
            Teen,
            Adult,
            Leader
        }

        public VillagerType Type = VillagerType.Teen;

        [Header("Locomotion Settings")]
        public float MoveSpeed = 3f;
        public float StopDistance = 0.1f;
        public float RotationSpeed = 5f;
        public bool UsePhysicsMovement = true;

        [Header("Pathfinding Settings")]
        public bool UseDynamicRepathing = false;
        public float RepathRate = 1f;

        [Header("Ground Detection Settings")]
        public LayerMask DetectionLayer;
        public float DetectionRadius = 0.1f;

        [Header("Sensor Settings")]
        public List<string> Tags = new List<string>();
        public float SensorRadius = 10f;
    }
}
