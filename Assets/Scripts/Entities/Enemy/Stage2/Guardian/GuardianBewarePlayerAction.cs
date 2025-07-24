using System;
using ToB.Entities;
using ToB.Utils;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GuardianBewarePlayer", story: "[Self] beware Player", category: "Action", id: "e9303507d62254fcc67c72a97ad75c14")]
public partial class GuardianBewarePlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Guardian> Guardian;
    [SerializeReference] public BlackboardVariable<String> MoveKey;

    private bool prevDetected;
    private float stateChangeTime;
    private float stateChangeTimeThreshold = 0.1f;
    //private Guardian owner;
    
    protected override Status OnStart()
    {   
        if(Time.time - stateChangeTime < stateChangeTimeThreshold) return Status.Success;
        if (Guardian.Value.AttackableAreaInnerSensor.TargetInArea && Guardian.Value.AttackableAreaOuterSensor.TargetInArea)
        {
            Guardian.Value.Physics.externalVelocity[MoveKey] =
                Guardian.Value.TargetDirectionHorizontal * -1 * Guardian.Value.GuardianSO.MoveSpeed;
        }
        else if (!Guardian.Value.AttackableAreaInnerSensor.TargetInArea && !Guardian.Value.AttackableAreaOuterSensor.TargetInArea)
        {
            Guardian.Value.Physics.externalVelocity[MoveKey] =
                Guardian.Value.TargetDirectionHorizontal * Guardian.Value.GuardianSO.MoveSpeed;
        }
        else
        {
            Guardian.Value.Physics.externalVelocity[MoveKey] = Vector2.zero;
        }
        
        // 이동 방향 낭떠러지 검사
        if (Guardian.Value.Physics.IsLedgeOnSide(
                DirectionUtil.GetHorizontalDirection(Guardian.Value.Physics.externalVelocity[EnemyPhysicsKeys.MOVE]
                )))
        {
            Guardian.Value.Physics.externalVelocity[MoveKey] = Vector2.zero;
        }
        stateChangeTime = Time.time;
        
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

