using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RecordCurrentTime", story: "Record Current Time at [Here]", category: "Action", id: "ae29e227ce67046cfafe969715f5808a")]
public partial class RecordCurrentTimeAction : Action
{
    [SerializeReference] public BlackboardVariable<float> Here;

    protected override Status OnStart()
    {
        Here.Value = Time.time;
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

