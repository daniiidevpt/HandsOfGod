using HOG.Resources;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CollectResource", story: "Agent collects [Resource]", category: "Action", id: "4fe77974804ab9ccffe0dfad1f8aebeb")]
public partial class CollectResourceAction : Action
{
    [SerializeReference] public BlackboardVariable<Resource> Resource;

    private Resource m_Resource;

    protected override Status OnStart()
    {
        m_Resource = Resource.Value;

        if (m_Resource is Wood)
        {
            var wood = m_Resource as Wood;
            if (wood.IsRegrowing)
            {
                return Status.Failure;
            }
        }

        return Status.Running;
    }
     
    protected override Status OnUpdate()
    {
        if (m_Resource is Wood)
        {
            var wood = m_Resource as Wood;
            if (wood.IsRegrowing)
            {
                return Status.Failure;
            }
            else
            {
                m_Resource.Collect();
                return Status.Success;
            }
        }
        
        if (m_Resource is not Wood)
        {
            m_Resource.Collect();
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        m_Resource = null;
    }
}

