using HOG.Building;
using HOG.Resources;
using UnityEngine;

namespace HOG.Villager
{
    public class StateContext
    {
        public Transform Target;
        public Resource TargetResource;
        public HouseBuilder TargetBuilding;
        public Vector3? TargetPosition;
    }

}
