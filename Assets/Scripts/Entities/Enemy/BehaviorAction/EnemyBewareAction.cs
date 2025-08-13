using System;
using System.Collections.Generic;
using ToB.Entities;
using ToB.Utils;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EnemyBeware", story: "[Enemy] Beware [Target] distance [min] ~ [max]", category: "Action", id: "a9127df294c70b048bc06aed390d3413")]
public partial class EnemyBewareAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    [SerializeReference] public BlackboardVariable<float> Min;
    [SerializeReference] public BlackboardVariable<float> Max;
    
    Dictionary<string, Vector2> Velocities => Enemy.Value.Physics.externalVelocity;

    protected override Status OnStart()
    {
        float distanceSQR = Enemy.Value.GetTargetDistanceSQR();

        if (distanceSQR < Min.Value * Min.Value)
        {
            MoveToTarget(true);
        }
        else if (distanceSQR > Max.Value * Max.Value)
        {
            MoveToTarget();
        }
        else
        {
            Velocities[EnemyPhysicsKeys.MOVE] = Vector2.zero;
        }
        
        return Status.Running;
    }


    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
    private void MoveToTarget(bool opposite = false)
    {
        Vector2 targetDirection = Enemy.Value.GetTargetDirection();
        Enemy.Value.LookTarget();
        
        if (opposite) targetDirection = -targetDirection;
        

        if (Enemy.Value.Physics.IsLedgeOnSide(targetDirection))
            Velocities[EnemyPhysicsKeys.MOVE] = Vector2.zero;
        else
        {
            Velocities[EnemyPhysicsKeys.MOVE] = 
                DirectionUtil.GetHorizontalDirection(targetDirection) * ((IEnemyGroundMoveSO)Enemy.Value.EnemySO).MoveSpeed;
        }
    }
}

