
using System;
using UnityEngine;

namespace ToB.Entities
{
    public class SecurityArcherFSM : EnemyStrategy
    {
        SecurityArcher Owner => enemy as SecurityArcher;
        
        public SecurityArcherMovePattern movePattern;
        public GroundDefaultChasePattern groundChasePattern;
        public SecurityArcherPerimeterPattern perimeterPattern;
        public HitReactionPattern hitReactionPattern;
        public AnimationBasedAttackPattern normalAttackPattern;
        public SecurityArcherDodgePattern dodgePattern;

        private bool hasDamagedCurrentFrame;
        public float lastSpecialAttackTime; 
        public bool siegeTrigger;
        public override void Init()
        {
            movePattern = new SecurityArcherMovePattern(this);
            groundChasePattern = new GroundDefaultChasePattern(this);
            perimeterPattern = new SecurityArcherPerimeterPattern(this);
            hitReactionPattern = new HitReactionPattern(this);
            normalAttackPattern = new AnimationBasedAttackPattern(this);  
            dodgePattern = new SecurityArcherDodgePattern(this);
                
            movePattern.AddTransition(()=>enemy.target, groundChasePattern, ()=>enemy.Animator.SetBool(EnemyAnimationString.Chase, true));
            perimeterPattern.AddTransition(()=>enemy.target, groundChasePattern,()=>enemy.Animator.SetBool(EnemyAnimationString.Chase, true));
            groundChasePattern.AddTransition(()=>!enemy.target, perimeterPattern,()=>enemy.Animator.SetBool(EnemyAnimationString.Chase, false));
            groundChasePattern.AddTransition(NormalAttackCondition, normalAttackPattern, () => enemy.Knockback.isActive = false);
            
            movePattern.AddTransition(SpecialAttackCondition, dodgePattern, () => enemy.Knockback.isActive = false);
            perimeterPattern.AddTransition(SpecialAttackCondition, dodgePattern, () => enemy.Knockback.isActive = false);
            groundChasePattern.AddTransition(SpecialAttackCondition, dodgePattern, () => enemy.Knockback.isActive = false);
            
            normalAttackPattern.AddTransition(() => enemy.Animator.GetBool(EnemyAnimationString.AttackEnd),
                groundChasePattern, () => enemy.Knockback.isActive = true);
            
            dodgePattern.AddTransition(() => enemy.Animator.GetBool(EnemyAnimationString.AttackEnd),
                groundChasePattern, () => enemy.Knockback.isActive = true);
            
            Owner.Stat.OnTakeDamage += PlantDamagedFlag;
            lastSpecialAttackTime = float.MinValue;
            ChangePattern(movePattern);
        }

        protected override void Update()
        {
            if (NotSuperArmor() && hasDamagedCurrentFrame && Owner.IsAlive)
            {
                ChangePattern(hitReactionPattern);
                hasDamagedCurrentFrame = false;
                return;
            }
            hasDamagedCurrentFrame = false;
            base.Update();
        }

        private bool NotSuperArmor()
        {
            return currentPattern == movePattern 
                || currentPattern == perimeterPattern
                || currentPattern == groundChasePattern;
        }
        
        // 모든 적이 몸이 단 하나의 피격주체라고 감안하지 않고 있어서 FSM 단에서 일단 피격 플래그를 만들어둡니다 
        private void PlantDamagedFlag()
        {
            hasDamagedCurrentFrame = true;
        }

        public override void ChangeToDefaultPattern()
        {
            base.ChangeToDefaultPattern();
            ChangePattern(perimeterPattern);
        }

        private void OnDisable()
        {
            Owner.Stat.OnTakeDamage -= PlantDamagedFlag;
        }

        private bool NormalAttackCondition()
        {
            if (!Owner.target) return false;
            if (!Owner.NormalAttackSensor.TargetInArea) return false;
            if(SpecialAttackCondition()) return false;
            return true;
        }
        private bool SpecialAttackCondition()
        {
            if (Time.time - lastSpecialAttackTime < 5) return false;
            if (!Owner.DodgeSensor.TargetInArea) return false;
            return true;
        }
    }
    
}
