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
        }

        IEnumerator Dig()
        {
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
            
            yield return new WaitForSeconds(Random.Range(0f,1f));
            
            // TODO : 지면에 출몰 예정 표시
            
            yield return new WaitForSeconds(1f);
            
            float ascendHeightPower = 25;
            
            enemy.Physics.velocityY = ascendHeightPower;
            enemy.Physics.gravityEnabled = true;
            strategy.Sprite.flipX = enemy.GetTargetDirection().x < 0;
            
            yield return new WaitForSeconds(0.8f);
            coroutine = enemy.StartCoroutine(Tackle());
        }

        IEnumerator Tackle()
        {
            Vector2 direction = (Vector2)enemy.target.transform.position - (Vector2)enemy.transform.position;
            RaycastHit2D hit = Physics2D.Raycast(enemy.transform.position, direction.normalized, 100f, strategy.GroundLayer);
            
            Vector2 destination = hit.point;
            enemy.Physics.gravityEnabled = false;
            enemy.Physics.collisionEnabled =true;
            
            float tackleSpeed = 30;
            while (!enemy.IsGrounded)
            {
                Vector2 currentPosition = enemy.transform.position;
                Vector2 moveDir = (destination - currentPosition).normalized;
                enemy.Physics.velocity = moveDir * tackleSpeed;
                yield return null;
            }
            enemy.Animator.SetBool(EnemyAnimationString.Roll, false);
            yield return new WaitForSeconds(0.025f);    // 충돌 후 벽에 끼인 프레임 임시 처리. 물리 처리 바꿀 예정

            enemy.Physics.velocity = new Vector2(0, 10);
            Debug.Log("태클  후 속도 변환 : " + Time.frameCount );
            enemy.Animator.SetBool(EnemyAnimationString.Jump, true);
            
            enemy.Physics.gravityEnabled = true;

            yield return new WaitUntil(() => enemy.IsGrounded);
            enemy.Animator.SetBool(EnemyAnimationString.Jump, false);
            
            
            Exit();
        }
    }
}