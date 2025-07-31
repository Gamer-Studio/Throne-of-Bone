using System;
using ToB.Entities;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SentinelBloodBubble", story: "[Sentinel] Blood Bubble", category: "Action", id: "69bd022a7d2d0418aaf2272f27791c27")]
public partial class SentinelBloodBubbleAction : Action
{
    [SerializeReference] public BlackboardVariable<Sentinel> Sentinel;

    protected override Status OnStart()
    {
        Sentinel.Value.StartBubbleAttack();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Sentinel.Value.BubbleAttackEnd ? Status.Success : Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

