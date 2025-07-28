using System;
using ToB.Entities;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "SentinelCheckRange", story: "[Sentinel] consider Ranged Attack", category: "Conditions", id: "9e9717e0011cbb38bba9e96434352040")]
public partial class SentinelCheckRangeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Sentinel> Sentinel;

    public override bool IsTrue()
    {
        if (Time.time > Sentinel.Value.lastRangeAttackTime + Sentinel.Value.rangeAttackCooldown) return false;
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
