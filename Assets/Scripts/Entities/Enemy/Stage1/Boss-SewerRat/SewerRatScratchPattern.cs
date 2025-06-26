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
            strategy.ScratchEffect.gameObject.SetActive(false);

        }

        IEnumerator Dash()
        {
            if(!enemy.target) yield break;
            
            enemy.Animator.SetBool("Roll", true);
            
            enemy.bodyDamage = 20;  // 대쉬 시 충돌 데미지
            Vector2 dashDirection = enemy.GetTargetDirection();
            dashDirection.y = 0;
            dashDirection.Normalize();

            float dashSpeed = 20f;
            float dashDuration = 0.3f;
            
            enemy.Physics.velocity = dashDirection * dashSpeed;
            
            yield return new WaitForSeconds(dashDuration);

            enemy.Physics.velocity = new Vector2(0, enemy.Physics.velocityY);

            coroutine = enemy.StartCoroutine(Scratch());
            enemy.Animator.SetBool("Roll", false);
            enemy.bodyDamage = enemy.EnemyData.ATK;
        }

        IEnumerator Scratch()
        {
            // TODO : 스크래치 애니메이션과 함께 판정 처리
            enemy.Animator.SetBool("Bark", true);
            strategy.ScratchEffect.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.59f); // 애니메이션 클립 시간 
            enemy.Animator.SetBool("Bark", false);
            Exit();
            yield return null;
        }
    }
}