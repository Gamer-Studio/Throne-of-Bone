using System;
using ToB.Entities;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DeleteExternalVelocity", story: "Delete External Velocity [Key]", category: "Action", id: "48be7343919df7a47f599f15e52352e4")]
public partial class DeleteExternalVelocityAction : Action
{
    [SerializeReference] public BlackboardVariable<string> Key;
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;
    
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
        Enemy.Value.Physics.externalVelocity.Remove(Key);
    }
}

