using System;
using ToB;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "GuardianShieldCondition", story: "[GuardianShieldArea] detected danger", category: "Conditions", id: "6be851aadc043077e008fc71da0f4aa2")]
public partial class GuardianShieldCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GuardianShieldSensor> GuardianShieldArea;

    public override bool IsTrue()
    {
        return !GuardianShieldArea.Value.Entered;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
