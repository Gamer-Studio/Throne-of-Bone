using System;
using ToB.Utils;
using UnityEngine;

namespace ToB.Entities
{
    public class SecurityArcherDodgePattern:EnemyPattern
    {
        private readonly SecurityArcher owner;
        private readonly SecurityArcherFSM fsm;

        private bool siege;
        public SecurityArcherDodgePattern(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
            owner = enemy as SecurityArcher;
            fsm = strategy as SecurityArcherFSM;
        }

        public override void Enter()
        {
            base.Enter();
            enemy.Animator.SetTrigger(EnemyAnimationString.SpecialAttack);
            int randomSign = UnityEngine.Random.value < 0.5f ? -1 : 1;
            enemy.Physics.externalVelocity[EnemyPhysicsKeys.MOVE] = new Vector2(randomSign * owner.DataSO.DodgeMoveSpeed, 0);
            StopOnLedge();
            
            owner.LookHorizontal(DirectionUtil.GetHorizontalDirection(randomSign));
            owner.Hitbox.enabled = false;
            fsm.lastSpecialAttackTime = Time.time;
            
            owner.DodgeEffect.gameObject.SetActive(true);
            var main = owner.DodgeEffect.main;
            main.startRotationX = randomSign < 0 ? -1.5f : 1.5f;
            
            Vector3 dodgeEffectOffset = owner.dodgeEffectOffset;
            //dodgeEffectOffset.x *= randomSign;
            owner.DodgeEffect.transform.position = owner.transform.position + dodgeEffectOffset;
            owner.DodgeEffect.Play();

            siege = false;
        }

        public override void Execute()
        {
            base.Execute();

            if (!siege)
            {
               StopOnLedge();
            }

            if (fsm.siegeTrigger)
            {
                owner.LookHorizontal(owner.LookDirectionHorizontal * -1);
                owner.Hitbox.enabled = true;
                enemy.Physics.externalVelocity.Remove(EnemyPhysicsKeys.MOVE);
                fsm.siegeTrigger = false;
                siege = true;
            }
        }

        public override void Exit()
        {
            base.Exit();
            owner.DodgeEffect.gameObject.SetActive(false);
            enemy.Physics.externalVelocity.Remove(EnemyPhysicsKeys.MOVE); 
        }
         
        public void StopOnLedge()
        {
            Vector2 moveDirection =
                DirectionUtil.GetHorizontalDirection(enemy.Physics.externalVelocity[EnemyPhysicsKeys.MOVE].x);

            if (moveDirection != Vector2.zero && enemy.Physics.IsLedgeOnSide(moveDirection))
            {
                enemy.Physics.externalVelocity[EnemyPhysicsKeys.MOVE] = Vector2.zero;
            }
        }
    }
   
}