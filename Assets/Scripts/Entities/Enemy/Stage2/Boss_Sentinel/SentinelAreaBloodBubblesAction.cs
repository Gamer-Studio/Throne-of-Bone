using System;
using ToB.Entities;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SentinelAreaBloodBubbles", story: "[SentinelArea] BloodBubbles", category: "Action", id: "41edf4b124d449f235f07b05b2e12080")]
public partial class SentinelAreaBloodBubblesAction : Action
{
    [SerializeReference] public BlackboardVariable<SentinelArea> SentinelArea;

    protected override Status OnStart()
    {
        SentinelArea.Value.StartBloodBubbles();
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

