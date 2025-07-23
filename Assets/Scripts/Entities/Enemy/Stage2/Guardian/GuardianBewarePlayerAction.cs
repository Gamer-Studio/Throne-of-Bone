using System;
using ToB.Entities;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GuardianBewarePlayer", story: "[Self] beware Player", category: "Action", id: "e9303507d62254fcc67c72a97ad75c14")]
public partial class GuardianBewarePlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    private Guardian owner;
    
    protected override Status OnStart()
    {
        if (!owner)
        {
            owner = Self.Value.GetComponent<Guardian>();
        }
        return Status.Running;
        
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

