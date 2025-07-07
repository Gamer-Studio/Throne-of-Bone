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

            if (!enemy.target || !owner.AttackSensor.TargetInArea)
            {
                if (enemy.Physics.externalVelocity[ROLL_KEY] == Vector2.zero)
                    fsm.ChangePattern(fsm.idleState);
                else
                {
                    Decelerate();
                }

                return;
            }


            if (enemy.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash != ROLL_STATE_HASH)
            {
                return;
            }

            enemy.LookTarget();

            Vector2 targetDirHorizontal = enemy.TargetDirectionHorizontal;

            // 진행 방향 낭떠러지 감지시 더 가지 않기
            if (enemy.Physics.externalVelocity[ROLL_KEY].x != 0 &&
                enemy.Physics.IsLedgeOnSide(enemy.Physics.externalVelocity[ROLL_KEY].x > 0 ? Vector2.right : Vector2.left))
            {
                enemy.Physics.externalVelocity[ROLL_KEY] = Vector2.zero;
            }
            else
            {
                // 타겟과 본인의 x값이 너무 가까우면 타겟과 x위치 통일
                if (Mathf.Abs(owner.RangeBaseSightSensor.TargetRB.position.x - enemy.transform.position.x) <
                    owner.DataSO.MoveSpeedWhileRoll * Time.fixedDeltaTime)
                {
                    enemy.Physics.externalVelocity[ROLL_KEY] =
                        new Vector2(owner.RangeBaseSightSensor.TargetRB.position.x - enemy.transform.position.x, 0);
                }
                else
                {
                    enemy.Physics.externalVelocity[ROLL_KEY] = targetDirHorizontal * owner.DataSO.MoveSpeedWhileRoll;
                }
            }
        }

        private void Decelerate()
        {
            if (enemy.Physics.externalVelocity[ROLL_KEY].x != 0 &&
                enemy.Physics.IsLedgeOnSide(enemy.Physics.externalVelocity[ROLL_KEY].x > 0 ? Vector2.right : Vector2.left))
            {
                enemy.Physics.externalVelocity[ROLL_KEY] = Vector2.zero;
                return;
            }
            Vector2 currentVelocity = enemy.Physics.externalVelocity[ROLL_KEY];

            int sign = currentVelocity.x > 0 ? -1 : 1;

            currentVelocity.x += sign * owner.DataSO.DecelerationSpeed * Time.deltaTime;
            
            if (currentVelocity.x < 0
                != enemy.Physics.externalVelocity[ROLL_KEY].x < 0)
            {
                currentVelocity.x = 0;
            }
            
            enemy.Physics.externalVelocity[ROLL_KEY] = currentVelocity;
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