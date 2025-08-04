using System;
using ToB.Entities;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FacePlayer", story: "Face Player", category: "Action", id: "35642e6a018d0d7ce8a341a640132917")]
public partial class FacePlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Enemy.Value.LookTarget();
        return Status.Success;
        //return Enemy.Value.target ? Status.Running : Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

