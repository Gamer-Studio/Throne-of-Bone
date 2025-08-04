using System;
using ToB.Entities;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetSentinelRangeAttackCooldown", story: "[Sentinel] Set Range Attack Cooldown [time]", category: "Action", id: "7b274c250913cb233a2725910b63dbd4")]
public partial class SetSentinelRangeAttackCooldownAction : Action
{
    [SerializeReference] public BlackboardVariable<Sentinel> Sentinel;
    [SerializeReference] public BlackboardVariable<float> Time;
    protected override Status OnStart()
    {
        Sentinel.Value.lastRangeAttackTime = UnityEngine.Time.time;
        Sentinel.Value.rangeAttackCooldown = Time;
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

