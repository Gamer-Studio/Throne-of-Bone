using System;
using Unity.Behavior;
using UnityEngine;
using Modifier = Unity.Behavior.Modifier;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RandomChance", story: "Execute Randomly with a [x] percent chance", category: "Flow", id: "1514b3f3400f397e13867401095448c2")]
public partial class RandomChanceModifier : Modifier
{
    [SerializeReference] public BlackboardVariable<float> X;

    protected override Status OnStart()
    {
        bool shouldExecuteChild = UnityEngine.Random.value < X.Value / 100f;

        if(!shouldExecuteChild)
        {
            return Status.Failure;
        }
    
        StartNode(Child);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Status childStatus = Child.CurrentStatus;

        return childStatus;
    }
}

