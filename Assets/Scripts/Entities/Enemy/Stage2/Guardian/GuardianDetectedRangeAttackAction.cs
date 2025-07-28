using System;
using ToB.Entities;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GuardianDetectedRangeAttack", story: "[Guardian] Detected Player Range Attack", category: "Action", id: "3e2aeb060fa32532089602405144b763")]
public partial class GuardianDetectedRangeAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<Guardian> Guardian;

    protected override Status OnStart()
    {
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

