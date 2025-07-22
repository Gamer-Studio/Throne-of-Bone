using System;
using System.Collections;
using UnityEngine;

namespace ToB.Entities
{
    public class SecurityGuardAttackPattern:EnemyPattern
    {
        private readonly SecurityGuard owner;
        private EnemySimpleSensor Sensor => owner.AttackSensor;
        private Coroutine attackCoroutine;
        private readonly SecurityGuardFSM fsm;
        
        public SecurityGuardAttackPattern(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
            owner = enemy as SecurityGuard;
            fsm = strategy as SecurityGuardFSM;
        }

        public override void Enter()
        {
            base.Enter();
            enemy.Animator.SetBool(EnemyAnimationString.Attack, true);
        }

        public override void Execute()
        {
            base.Execute();

            if (!Sensor.TargetInArea)
            {
                enemy.Animator.SetBool(EnemyAnimationString.Attack, false);
            }
            else
            {
                enemy.Animator.SetBool(EnemyAnimationString.Attack, true);
            }
            

          
        }

        public override void LateExecute()
        {
            base.LateExecute();
            // AttackEnd 파라미터 수정은 StateMachineBehaviour로부터
            if (enemy.Animator.GetBool(EnemyAnimationString.AttackEnd))
            {
                enemy.StartCoroutine(LateMoveCachedPosition());
                fsm.ChangePattern(fsm.groundMovePattern);   
            }
        }

        public override void Exit()
        {
            base.Exit();
            enemy.Animator.SetBool(EnemyAnimationString.Attack, false);
        }

        IEnumerator LateMoveCachedPosition()
        {
            yield return null;
            enemy.transform.position = fsm.SavePosition.position;
        }
    }
}