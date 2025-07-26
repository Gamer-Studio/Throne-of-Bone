using System;
using DG.Tweening;
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
    [SerializeReference] public BlackboardVariable<float> LastTeleportTime;

    private int TeleportHash = Animator.StringToHash("Teleport");
    protected override Status OnStart()
    {
        Debug.Log("TPTime : "+ Time.time);
        LastTeleportTime.Value = Time.time;
        Guardian.Value.Animator.SetTrigger(TeleportHash);
        Guardian.Value.Hitbox.enabled = false;
        Guardian.Value.TeleportInvincible = DOVirtual.DelayedCall(2, ()=>Guardian.Value.Hitbox.enabled = true);
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

