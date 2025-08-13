using System;
using ToB.Entities;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "AttackAreaCheck", story: "[guardian] detect target in AttackArea", category: "Conditions", id: "830bf0dec004e3c417e4577f8ad27e5a")]
public partial class GuardianAttackAreaCheckCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Guardian> Guardian;
    
    public override bool IsTrue()
    {
        return Guardian.Value.AttackableAreaOuterSensor.TargetInArea;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
