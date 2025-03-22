using HOG.Resources;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "IsVillageLowOnResources", story: "Village is low on resources", category: "Conditions", id: "d834f25622339d28bf754984e34c77c5")]
public partial class IsVillageLowOnResourcesCondition : Condition
{
    public override bool IsTrue()
    {
        var manager = ResourceManager.Instance;

        if (manager.GetResourceAmount(ResourceType.Wood) < 10)
        {
            return true;
        }

        return false;
    }

    public override void OnStart()
    {

    }

    public override void OnEnd()
    {
    }
}
