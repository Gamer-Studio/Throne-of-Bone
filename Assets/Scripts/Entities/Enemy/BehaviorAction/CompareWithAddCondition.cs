using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CompareWithAdd", story: "[A] is lower than [B] + [C]", category: "Conditions", id: "e91cb972f528918f7b1fe3e013d0b563")]
public partial class CompareWithAddCondition : Condition
{
    [SerializeReference] public BlackboardVariable<float> A;
    [Comparison(comparisonType: ComparisonType.All)]
    [SerializeReference] public BlackboardVariable<ConditionOperator> Operator;
    [SerializeReference] public BlackboardVariable<float> B;
    [SerializeReference] public BlackboardVariable<float> C;

    public override bool IsTrue()
    {
        Debug.Log(A.Value + " " + B.Value + " " + C.Value);
        return A.Value < B.Value + C.Value;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
