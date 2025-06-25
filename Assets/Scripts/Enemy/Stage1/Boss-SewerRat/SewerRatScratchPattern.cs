using System;
using System.Collections;
using UnityEngine;

namespace ToB
{
    public class SewerRatScratchPattern : SewerRatPattern
    {
        Coroutine coroutine;
        /// <inheritdoc/>
        public SewerRatScratchPattern(Enemy enemy, SewerRatStrategy strategy, Action EndCallback) : base(enemy, strategy, EndCallback)
        {
        }

        public override void Enter()
        {
            coroutine = enemy.StartCoroutine(Dash());
        }

        public override void Execute()
        {

        }

        protected override void Exit()
        {
            base.Exit();
            if(coroutine != null) enemy.StopCoroutine(coroutine);
        }

        IEnumerator Dash()
        {
            if(!enemy.target) yield break;
            
            enemy.Animator.SetTrigger(EnemyAnimationString.Dash);
            
            enemy.bodyDamage = 20;  // 대쉬 시 충돌 데미지
            Vector2 dashDirection = enemy.GetTargetDirection();
            dashDirection.y = 0;

            float dashSpeed = 10f;
            float dashDuration = 0.3f;
            
            enemy.rb.linearVelocity = dashDirection * dashSpeed;
            
            yield return new WaitForSeconds(dashDuration);

            enemy.rb.linearVelocity = new Vector2(0, enemy.rb.linearVelocity.y);

            coroutine = enemy.StartCoroutine(Scratch());
            enemy.bodyDamage = enemy.EnemyData.ATK;
        }

        IEnumerator Scratch()
        {
            // TODO : 스크래치 애니메이션과 함께 판정 처리
            enemy.Animator.SetBool("Bark", true);
            yield return new WaitForSeconds(0.59f); // 애니메이션 클립 시간 
            enemy.Animator.SetBool("Bark", false);
            
            Exit();
            yield return null;
        }
    }
}