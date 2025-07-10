using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ToB.Entities
{
    public class SewerRatDigPattern : SewerRatPattern
    {
        Coroutine coroutine;

        public SewerRatDigPattern(EnemyStrategy strategy, Action EndCallback) : base(strategy, EndCallback)
        {
        }

        /// <inheritdoc/>
        public override void Enter()
        {
            coroutine = enemy.StartCoroutine(Dig());
        }

        public override void Execute()
        {
           
        }

        public override void Exit()
        {
            base.Exit();
            if(coroutine != null) enemy.StopCoroutine(coroutine);
            ratStrategy.GroundDustEffect.gameObject.SetActive(false);
            ratStrategy.GroundRubble.gameObject.SetActive(false);
        }

        IEnumerator Dig()
        {
            sewerRat.Sprite.sortingOrder = -100;
            sewerRat.EnemyBody.ChangeDamage(sewerRat.DataSO.RollDamage);;
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
            enemy.transform.position = ratStrategy.ascendLocation.GetRandomPosition(fixedY:true);

            Vector2 groundPoint = GetGroundPoint();
            ratStrategy.GroundDustEffect.transform.position = groundPoint;
            ratStrategy.GroundRubble.transform.position = groundPoint;
            
            
            yield return new WaitForSeconds(Random.Range(0f,1f));
            
            ratStrategy.GroundDustEffect.gameObject.SetActive(true);
            ratStrategy.GroundDustEffect.Play();
            
            yield return new WaitForSeconds(1f);
            
            ratStrategy.GroundRubble.gameObject.SetActive(true);
            ratStrategy.GroundRubble.Play();
            
            enemy.Physics.velocityY = sewerRat.DataSO.AscendPower;
            enemy.Physics.gravityEnabled = true;
            ratStrategy.LookPlayer();
           
            coroutine = enemy.StartCoroutine(Tackle());
        }

        IEnumerator Tackle()
        {
            yield return new WaitForSeconds(0.4f);
            sewerRat.Sprite.sortingOrder = 700;
            
            Vector2 direction = (Vector2)enemy.target.transform.position - (Vector2)enemy.transform.position;
            RaycastHit2D hit = Physics2D.Raycast(enemy.transform.position, direction.normalized, 100f, ratStrategy.GroundLayer);
            
            Vector2 destination = hit.point;
            
            yield return new WaitForSeconds(0.4f);
            
            enemy.Physics.gravityEnabled = false;
            enemy.Physics.collisionEnabled =true;
            
            float tackleSpeed = sewerRat.DataSO.TackleSpeed;
            
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

            sewerRat.EnemyBody.ChangeDamage(sewerRat.DataSO.BodyDamage);
            Exit();
        }
        
        private Vector2 GetGroundPoint()
        {
            Vector2 rayOrigin = enemy.transform.position + new Vector3(0, 5f, 0);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 100f, ratStrategy.GroundLayer);
            return hit.point;
        }
    }
}