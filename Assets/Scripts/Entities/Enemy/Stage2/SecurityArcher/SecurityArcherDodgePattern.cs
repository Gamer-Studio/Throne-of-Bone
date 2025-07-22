using System;
using ToB.Utils;
using UnityEngine;

namespace ToB.Entities
{
    public class SecurityArcherDodgePattern:EnemyPattern
    {
        private readonly SecurityArcher owner;
        private readonly SecurityArcherFSM fsm;
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
        }

        public override void Execute()
        {
            base.Execute();

            if (fsm.siegeTrigger)
            {
                owner.LookHorizontal(owner.LookDirectionHorizontal * -1);
                owner.Hitbox.enabled = true;
                enemy.Physics.externalVelocity.Remove(EnemyPhysicsKeys.MOVE);
                fsm.siegeTrigger = false;
            }
        }

        public override void Exit()
        {
            base.Exit();
            owner.DodgeEffect.gameObject.SetActive(false);
            enemy.Physics.externalVelocity.Remove(EnemyPhysicsKeys.MOVE); 
        }
    }
}