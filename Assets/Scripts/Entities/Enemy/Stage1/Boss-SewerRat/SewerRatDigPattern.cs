using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ToB
{
    public class SewerRatDigPattern : SewerRatPattern
    {
        Coroutine coroutine;
        /// <inheritdoc/>
        public SewerRatDigPattern(Enemy enemy, SewerRatStrategy strategy, Action EndCallback) : base(enemy, strategy, EndCallback)
        {
        }

        public override void Enter()
        {
            coroutine = enemy.StartCoroutine(Dig());
        }

        public override void Execute()
        {
           
        }

        protected override void Exit()
        {
            base.Exit();
            if(coroutine != null) enemy.StopCoroutine(coroutine);
            strategy.GroundDustEffect.gameObject.SetActive(false);
            strategy.GroundRubble.gameObject.SetActive(false);
        }

        IEnumerator Dig()
        {
            enemy.bodyDamage = 20;
            enemy.Animator.SetBool(EnemyAnimationString.Roll, true);
            enemy.Physics.collisionEnabled = false;
            enemy.Physics.gravityEnabled = false;
            Tween tween = enemy.transform.DOShakePosition(1, new Vector3(0.5f, 0.5f, 0), 20, 90);
            yield return tween.WaitForCompletion();

            tween = enemy.transform.DOMoveY(enemy.transform.position.y - 5, 0.7f);
            yield return tween.WaitForCompletion();

            coroutine = enemy.StartCoroutine(Ascend());
        }

        IEnumerator Ascend()
        {
            enemy.transform.position = strategy.ascendLocation.GetRandomPosition(fixedY:true);

            Vector2 groundPoint = GetGroundPoint();
            strategy.GroundDustEffect.transform.position = groundPoint;
            strategy.GroundRubble.transform.position = groundPoint;
            
            
            yield return new WaitForSeconds(Random.Range(0f,1f));
            
            strategy.GroundDustEffect.gameObject.SetActive(true);
            strategy.GroundDustEffect.Play();
            
            yield return new WaitForSeconds(1f);
            
            strategy.GroundRubble.gameObject.SetActive(true);
            strategy.GroundRubble.Play();
            
            const float ascendHeightPower = 32;
            
            enemy.Physics.velocityY = ascendHeightPower;
            enemy.Physics.gravityEnabled = true;
            strategy.LookPlayer();
           
            coroutine = enemy.StartCoroutine(Tackle());
        }

        IEnumerator Tackle()
        {
            yield return new WaitForSeconds(0.4f);
            Vector2 direction = (Vector2)enemy.target.transform.position - (Vector2)enemy.transform.position;
            RaycastHit2D hit = Physics2D.Raycast(enemy.transform.position, direction.normalized, 100f, strategy.GroundLayer);
            
            Vector2 destination = hit.point;
            
            yield return new WaitForSeconds(0.4f);
            
            enemy.Physics.gravityEnabled = false;
            enemy.Physics.collisionEnabled =true;
            
            const float tackleSpeed = 40;
            
            Vector2 fixedDirection = (destination - (Vector2)enemy.transform.position).normalized;  // 단순 플레이어 방향 방식이 궤도 오차가 심했어서 레이로
            
            while (!enemy.Physics.HasCollided)
            {
                // 고정된 방향으로 등속 이동
                enemy.Physics.velocity = fixedDirection * tackleSpeed;
                yield return null;

            }
            enemy.Animator.SetBool(EnemyAnimationString.Roll, false);
            yield return new WaitForFixedUpdate();
            
            enemy.Physics.velocity = new Vector2(0, 10);
            enemy.Animator.SetBool(EnemyAnimationString.Jump, true);
            
            enemy.Physics.gravityEnabled = true;

            yield return new WaitUntil(() => enemy.IsGrounded);
            enemy.Animator.SetBool(EnemyAnimationString.Jump, false);

            enemy.bodyDamage = enemy.EnemyData.ATK;
            Exit();
        }
        
        private Vector2 GetGroundPoint()
        {
            Vector2 rayOrigin = enemy.transform.position + new Vector3(0, 5f, 0);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 100f, strategy.GroundLayer);
            return hit.point;
        }
    }
}