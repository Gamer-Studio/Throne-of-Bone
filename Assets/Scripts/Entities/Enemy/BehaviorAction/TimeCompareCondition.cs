using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "TimeCompare", story: "Time is less than [OriginTime] + [IntervalTime]", category: "Conditions", id: "0153d5626892a545a7baabde012871b5")]
public partial class TimeCompareCondition : Condition
{
    [SerializeReference] public BlackboardVariable<float> OriginTime;
    [SerializeReference] public BlackboardVariable<float> IntervalTime;

    public override bool IsTrue()
    {
        return Time.time < OriginTime.Value + IntervalTime.Value;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
