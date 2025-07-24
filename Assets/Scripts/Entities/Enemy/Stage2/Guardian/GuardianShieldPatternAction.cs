using System;
using DG.Tweening;
using ToB.Entities;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GuardianShieldPattern", story: "[Guardian] activate Shield", category: "Action", id: "f6ea3116ed9861296158f4e2999c2875")]
public partial class GuardianShieldPatternAction : Action
{
    [SerializeReference] public BlackboardVariable<Guardian> Guardian;
    [SerializeReference] public BlackboardVariable<bool> CanShield;

    private int ShieldHash = Animator.StringToHash("Shield");
    private float ShieldDuration => Guardian.Value.GuardianSO.ShieldDuration;

    private bool shieldEnd;
    
    Tween shieldActiveTween;
    
    protected override Status OnStart()
    {
        CanShield.Value = false;
        shieldEnd = false;
        Guardian.Value.ShieldRecharger = DOVirtual.DelayedCall(Guardian.Value.GuardianSO.ShieldRechargeTime, () =>
        {
            CanShield.Value = true;
        });
        shieldActiveTween = DOVirtual.DelayedCall(ShieldDuration, () =>
        {
            shieldEnd = true;
        });

        Guardian.Value.Animator.SetTrigger(ShieldHash);
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return shieldEnd ? Status.Success : Status.Running;
    }

    protected override void OnEnd()
    {
        shieldActiveTween.Kill();
    }
}

