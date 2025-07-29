using System;
using ToB.Entities;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "SentinelCheckRange", story: "[Sentinel] consider Ranged Attack : Distance [Distance] , Attack Time [LastRangeAttackTime]", category: "Conditions", id: "9e9717e0011cbb38bba9e96434352040")]
public partial class SentinelCheckRangeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Sentinel> Sentinel;
    [SerializeReference] public BlackboardVariable<float> Distance;
    [SerializeReference] public BlackboardVariable<float> LastRangeAttackTime;

    
    public override bool IsTrue()
    {
        if (Sentinel.Value.GetTargetDistanceSQR() < Distance.Value*Distance.Value) return true;
        
        if (Time.time > LastRangeAttackTime.Value + Sentinel.Value.rangeAttackCooldown) return false;
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
