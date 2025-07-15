using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ToB.Entities
{
    public class SewerRatToxicBonePattern : SewerRatPattern
    {
        Coroutine coroutine;

        public SewerRatToxicBonePattern(EnemyStrategy strategy, Action EndCallback) : base(strategy, EndCallback)
        {
        }

        /// <inheritdoc/>
     

        public override void Enter()
        {
            coroutine = enemy.StartCoroutine(StepBack());
        }

        public override void Execute()
        {

        }

        public override void Exit()
        {
            base.Exit();
            if(coroutine != null) enemy.StopCoroutine(coroutine);
        }

        // 동작 1. 뒤로 점프하여 거리를 벌린다
        IEnumerator StepBack()
        {
            Vector2 targetDirection = enemy.GetTargetDirection();
            Vector2 jumpDirection = new Vector2(0, 0);
            
            // 점프 방향은 플레이어 좌우만 체크하고 y는 1 
            jumpDirection.x = targetDirection.x > 0 ? -1 : 1;
            jumpDirection.y = 1;
            float jumpForce = 5;
            
            enemy.Physics.velocity = jumpDirection * jumpForce;

            enemy.Animator.SetBool(EnemyAnimationString.Jump, true);

            yield return new WaitForFixedUpdate();
            yield return new WaitUntil(() => enemy.IsGrounded);
            enemy.Animator.SetBool(EnemyAnimationString.Jump, false);
            
            enemy.Physics.velocity = Vector2.zero;
            
            coroutine = enemy.StartCoroutine(ThrowBone());
        }

        // 동작 2. 3회 뼈다귀를 던진다
        IEnumerator ThrowBone()
        {
            enemy.Animator.SetTrigger(EnemyAnimationString.MotionCancel);
            enemy.Animator.SetBool("Bark", true);
            for (int i = 0; i < 3; i++)
            {
                float speed = sewerRat.DataSO.ToxicBoneSpeed;
                
                // GameObject boneObj = Object.Instantiate(ratStrategy.ToxicBonePrefab);
                GameObject boneObj = ratStrategy.ToxicBonePrefab.Pooling();
                boneObj.transform.position = enemy.transform.position + new Vector3(0, 2, 0);
                ToxicBone bone = boneObj.GetComponent<ToxicBone>();         // 양이 많지 않아서 굳이 풀링하지 않고 있습니다.
                
                Vector2 direction = enemy.target.transform.position - boneObj.transform.position ;
                bone.LinearMovement.Init(direction, speed);
                bone.SimpleRotate.SetRotationSpeed(enemy.IsTargetLeft ? -360 : 360);
                
                yield return new WaitForSeconds(sewerRat.DataSO.ToxicBoneInterval);
            }

            enemy.Animator.SetBool("Bark", false);
            
            Exit();
        }
    }
}