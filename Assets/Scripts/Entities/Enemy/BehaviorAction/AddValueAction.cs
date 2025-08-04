using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AddValue", story: "[Target] Add [Value]", category: "Action", id: "7556efbd4c75ff4ff69e9d831b7eb5a5")]
public partial class AddValueAction : Action
{
    [SerializeReference] public BlackboardVariable<float> Target;
    [SerializeReference] public BlackboardVariable<float> Value;

    protected override Status OnStart()
    {
        Target.Value += Value.Value;
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

