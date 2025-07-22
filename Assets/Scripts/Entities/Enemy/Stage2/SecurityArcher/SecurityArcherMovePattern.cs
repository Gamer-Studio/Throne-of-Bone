using System;
using System.Collections;
using UnityEngine;

namespace ToB.Entities
{
    public class SecurityArcherMovePattern:GroundDefaultMovePattern
    {
        Coroutine moveTimerCoroutine;
        SecurityArcherFSM fsm => strategy as SecurityArcherFSM;
        SecurityArcherSO so => enemy.enemySO as SecurityArcherSO;
        
        public SecurityArcherMovePattern(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
        }

        public override void Enter()
        {
            base.Enter();
            moveTimerCoroutine = enemy.StartCoroutine(MoveTimer());
            enemy.Animator.SetBool(EnemyAnimationString.Move, true);
        }

        IEnumerator MoveTimer()
        {
            yield return new WaitForSeconds(so.MoveInterval);
            fsm.ChangePattern(fsm.perimeterPattern);
        }

        public override void Exit()
        {
            base.Exit();
            if(moveTimerCoroutine != null) enemy.StopCoroutine(moveTimerCoroutine);
            enemy.Animator.SetBool(EnemyAnimationString.Move, false);
        }
    }
}