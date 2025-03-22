using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/OnResourcesChanged")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "OnResourcesChanged", message: "OnResourcesChanged", category: "Events", id: "ab224d0d89cf43e4ea5b495907ff3459")]
public partial class OnResourcesChanged : EventChannelBase
{
    public delegate void OnResourcesChangedEventHandler();
    public event OnResourcesChangedEventHandler Event; 

    public void SendEventMessage()
    {
        Event?.Invoke();
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        Event?.Invoke();
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        OnResourcesChangedEventHandler del = () =>
        {
            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as OnResourcesChangedEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as OnResourcesChangedEventHandler;
    }
}

