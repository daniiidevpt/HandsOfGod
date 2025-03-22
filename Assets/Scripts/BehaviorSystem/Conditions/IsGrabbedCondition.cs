using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "IsGrabbed", story: "[Agent] is being [grabbed]", category: "Conditions", id: "300dff6c8bc773734043344b77b6a4fe")]
public partial class IsGrabbedCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<bool> Grabbed;

    public override bool IsTrue()
    {
        if (Grabbed)
            return true;
        else
            return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
