using System;
using ToB.Entities;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GuardianTeleport", story: "[guardian] Teleport", category: "Action", id: "28af6ac142cf1035aa184115073fcd3b")]
public partial class GuardianTeleportAction : Action
{
    [SerializeReference] public BlackboardVariable<Guardian> Guardian;

    private int TeleportHash = Animator.StringToHash("Teleport");
    protected override Status OnStart()
    {
        Guardian.Value.Animator.SetTrigger(TeleportHash);
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

