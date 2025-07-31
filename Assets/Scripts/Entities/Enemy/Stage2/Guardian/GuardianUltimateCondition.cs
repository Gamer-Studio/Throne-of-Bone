using System;
using ToB.Entities;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "GuardianUltimate", story: "[Guardian] can't use Ult", category: "Conditions", id: "2ba3273d8efc867086af87714d30d2b2")]
public partial class GuardianUltimateCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Guardian> Guardian;

    public override bool IsTrue()
    {
        return !Guardian.Value.CheckUltCondition();
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
