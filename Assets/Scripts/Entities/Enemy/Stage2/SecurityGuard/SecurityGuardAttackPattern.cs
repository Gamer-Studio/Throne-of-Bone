using System;
using System.Collections;
using UnityEngine;

namespace ToB.Entities
{
    public class SecurityGuardAttackPattern:EnemyPattern
    {
        private EnemySimpleSensor Sensor => ((SecurityGuard)enemy).AttackSensor;
        private Coroutine attackCoroutine;
        private SecurityGuardFSM fsm;
        
        public SecurityGuardAttackPattern(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
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
            
            if(!Sensor.TargetInArea)
                enemy.Animator.SetBool(EnemyAnimationString.Attack, false);

            // AttackEnd 파라미터 수정은 StateMachineBehaviour로부터
            if (enemy.Animator.GetBool(EnemyAnimationString.AttackEnd))
            {
                enemy.Animator.SetBool(EnemyAnimationString.AttackEnd, false);
                fsm.ChangePattern(fsm.groundMovePattern);   
            }
        }
    }
}