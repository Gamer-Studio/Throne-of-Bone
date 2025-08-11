using System;
using System.Collections;
using UnityEngine;


namespace ToB.Entities
{
    public class CagedSpiderSelfDestructPattern : EnemyPattern
    {
        CagedSpider cagedSpider;
        private readonly CagedSpiderSO cagedSpiderSO;
        private int enteredFrame;
        
        Coroutine delayCoroutine;
        bool actionStarted;
        public CagedSpiderSelfDestructPattern(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
            cagedSpider = enemy as CagedSpider;
            cagedSpiderSO = enemy.EnemySO as CagedSpiderSO;
        }
        public override void Enter()
        {
            enemy.audioPlayer.StopAll();
            actionStarted = false;
            delayCoroutine = enemy.StartCoroutine(Delay());
        }

        public override void FixedExecute()
        {
            base.FixedExecute();

            if (!actionStarted) return;
            if (enteredFrame == Time.frameCount) return;
            
            if (enemy.Physics.HasCollided 
                || enemy.IsGrounded
                || enemy.GetTargetDistanceSQR() < Mathf.Pow(cagedSpiderSO.ExplodeSensorRadius, 2))
            {
                cagedSpider.EnemyBody.gameObject.Damage(9999);
                enemy.audioPlayer.Play("Basic Explosion 1");
            }
        }

        public override void Exit()
        {
            base.Exit();
            if(delayCoroutine != null) enemy.StopCoroutine(delayCoroutine);
            enemy.Physics.externalVelocity.Remove(EnemyDefaultConstants.MoveKey);
        }

        IEnumerator Delay()
        {
            yield return new WaitForSeconds(0.3f);
            enemy.Physics.externalVelocity[EnemyDefaultConstants.MoveKey] = cagedSpiderSO.ChaseSpeed * 1.7f * enemy.LookDirectionHorizontal;
            enemy.Physics.velocity = new Vector2(0, cagedSpiderSO.JumpForce);
            enteredFrame = Time.frameCount;
            actionStarted = true;
        }
    }
}
