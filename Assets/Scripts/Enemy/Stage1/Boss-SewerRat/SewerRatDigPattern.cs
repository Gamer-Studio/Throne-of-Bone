using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

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
            yield return new WaitForSeconds(1f);
            float ascendHeightPower = 25;
            
            enemy.rb.linearVelocityY = ascendHeightPower;
            enemy.Physics.gravityEnabled = true;
            
            
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
                Vector2 currentPosition = enemy.rb.position;
                Vector2 moveDir = (destination - currentPosition).normalized;
                enemy.rb.linearVelocity = moveDir * tackleSpeed;
                yield return null;
            }
            enemy.rb.linearVelocity = new Vector2(0, 10);
            enemy.Physics.gravityEnabled = true;
            Exit();
        }
    }
}