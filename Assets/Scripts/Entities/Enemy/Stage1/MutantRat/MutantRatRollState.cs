using System;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

namespace ToB.Entities
{
    public class MutantRatRollState : EnemyPattern
    {
        private readonly MutantRat owner;
        private MutantRatFSM fsm;
        
        const string ROLL_KEY = "Roll";
        private static readonly int ROLL_STATE_HASH = Animator.StringToHash("Base Layer.Roll");
        public MutantRatRollState(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
            owner = enemy as MutantRat;
            fsm = strategy as MutantRatFSM;
        }

        public override void Enter()
        {
            base.Enter();
            enemy.Animator.SetBool(EnemyAnimationString.Roll, true);
            owner.Knockback.isActive = false;
            enemy.Physics.externalVelocity[ROLL_KEY] = Vector2.zero;
        }

        public override void Execute()
        {
            base.Execute();

            if (!enemy.target)
            {
                Debug.Log(owner.SightSensor.transform.position + " || " + owner.SightSensor.TargetInRange.position);;
                fsm.ChangePattern(fsm.idleState);
                return;
            }
  
            
            if (enemy.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash != ROLL_STATE_HASH)
            {
                return;
            }

            enemy.LookTarget();

            Vector2 targetDirHorizontal = enemy.TargetDirectionHorizontal;
            
            // 진행 방향 낭떠러지 감지시 더 가지 않기
            if (enemy.Physics.IsLedgeOnSide(targetDirHorizontal))
            {
                enemy.Physics.externalVelocity[ROLL_KEY] = Vector2.zero;
            }
            else
            {
                // 타겟과 본인의 x값이 너무 가까우면 타겟과 x위치 통일
                if (Mathf.Abs(enemy.target.transform.position.x - enemy.transform.position.x) < Mathf.Pow(owner.DataSO.MoveSpeedWhileRoll * Time.fixedDeltaTime, 2))
                {
                    ForcePositionXtoTarget();
                }
                else
                {
                    enemy.Physics.externalVelocity[ROLL_KEY] = targetDirHorizontal * owner.DataSO.MoveSpeedWhileRoll;
                }

            }
            
            
        }

        private void ForcePositionXtoTarget()
        {
            enemy.Physics.externalVelocity[ROLL_KEY] = Vector2.zero;
            
            Vector3 ownerPos = enemy.transform.position;
            ownerPos.x = enemy.target.position.x;
            enemy.transform.position = ownerPos;
        }

        public override void Exit()
        {
            base.Exit();
            enemy.Animator.SetBool(EnemyAnimationString.Roll, false);
            owner.Knockback.isActive = true;
            enemy.Physics.externalVelocity.Remove(ROLL_KEY);
        }
    }
}
