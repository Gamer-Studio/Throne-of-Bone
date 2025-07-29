using System;
using Unity.Behavior;
using UnityEngine;
using Composite = Unity.Behavior.Composite;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TestFlow", story: "test", category: "Flow", id: "712b8a92791a15b567549ea20344012e")]
public partial class TestFlowSequence : Composite
{
    [SerializeReference] public Node Port1;
    [SerializeReference] public Node Port2;

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

