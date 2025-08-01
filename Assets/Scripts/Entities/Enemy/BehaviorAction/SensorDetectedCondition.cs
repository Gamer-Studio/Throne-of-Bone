using System;
using ToB.Entities;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "SensorDetected", story: "[Sensor] didn't detect Danger", category: "Conditions", id: "e99d8202a21d7bb456113a47a4aad540")]
public partial class SensorDetectedCondition : Condition
{
    [SerializeReference] public BlackboardVariable<EnemySimpleSensor> Sensor;

    public override bool IsTrue()
    {
        return !Sensor.Value.entered;
    }
}
