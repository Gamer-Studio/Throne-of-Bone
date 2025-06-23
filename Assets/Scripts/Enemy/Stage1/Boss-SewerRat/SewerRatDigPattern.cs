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
            enemy.SetGravity(false);
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
            enemy.SetGravity(true);
            
            yield return new WaitForSeconds(0.8f);
            coroutine = enemy.StartCoroutine(Tackle());
        }

        IEnumerator Tackle()
        {
            Vector2 destination = enemy.target.transform.position;
            enemy.SetGravity(false);
            float tackleSpeed = 30;
            while (!enemy.IsGrounded)
            {
                Vector2 direction = destination - (Vector2)enemy.transform.position;
                direction = direction.normalized;
                enemy.rb.linearVelocity = direction * tackleSpeed;
                yield return null;
            }
            enemy.rb.linearVelocity = new Vector2(0, 10);
            enemy.SetGravity(true);
            Exit();
        }
    }
}