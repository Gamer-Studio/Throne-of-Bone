using System;
using ToB.Entities;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SentinelClone", story: "[Sentinel] Clone", category: "Action", id: "d658f61f031c475db55752e3c81b0016")]
public partial class SentinelCloneAction : Action
{
    [SerializeReference] public BlackboardVariable<Sentinel> Sentinel;

    protected override Status OnStart()
    {
        Sentinel.Value.ClonePattern();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Sentinel.Value.Stat.CurrentHP / (Sentinel.Value.DataSO.HP - Sentinel.Value.DataSO.HP_1Phase) > 0.4f)
        {
            return Status.Running;
        }

        Sentinel.Value.ClonePatternDisable();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

