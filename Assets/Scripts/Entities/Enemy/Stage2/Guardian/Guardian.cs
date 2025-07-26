using System;
using System.Collections.Generic;
using DG.Tweening;
using ToB.Entities.Interface;
using Unity.Behavior;
using UnityEngine;

namespace ToB.Entities
{
    public class Guardian : Enemy
    {
        public GuardianSO DataSO => enemySO as GuardianSO;
        [field:SerializeField] public EnemyBody EnemyBody { get; private set; }
        [field:SerializeField] public EnemyStatHandler Stat { get; private set; }
        [field:SerializeField] public GameObject ShieldAreaObject { get; private set; }
        [field:SerializeField] public EnemySimpleSensor AttackableAreaInnerSensor { get; private set; }
        [field:SerializeField] public EnemySimpleSensor AttackableAreaOuterSensor { get; private set; }
        [field:SerializeField] public GuardianShieldSensor ShieldSensor { get; private set; }
        
        [field:SerializeField] public EnemyAttackArea BlastArea { get; private set; }

        [field:SerializeField] public EnemyBeamAttack Laser { get; private set; }
        [field:SerializeField] public EnemyBeamAttack LaserRain { get; private set; }
        
        
        [SerializeField] private BehaviorGraphAgent agent;

        [SerializeField] private ParticleSystem deathImpact;

        public Queue<float> ultConditionQueue;

        public Tween ShieldRecharger;
        public Tween TeleportInvincible;
        public Tween DamagedJudge;
        
        protected override void Awake()
        {
            base.Awake();

            Laser.Init(DataSO.LaserTickDamage);
            LaserRain.Init(DataSO.LaserTickDamage);
            
            agent.BlackboardReference.SetVariableValue("ShieldCooldown", DataSO.ShieldRechargeTime);
            agent.BlackboardReference.SetVariableValue("TeleportCooldown", DataSO.TeleportRechargeTime);
            agent.BlackboardReference.SetVariableValue("BlastCooldown", DataSO.BlastRechargeTime);
            agent.BlackboardReference.SetVariableValue("LaserCooldown", DataSO.LaserRechargeTime);
            
            agent.BlackboardReference.SetVariableValue("DamagedJudgeTime", 0.4f);
            
        }

        protected override void OnEnable() 
        {
            base.OnEnable();
            
            EnemyBody.Init(this, DataSO.BodyDamage);
            Stat.Init(this, DataSO);
            Knockback.Init(this);
            
            BlastArea.Init(this, DataSO.BlastDamage, DataSO.BlastKnockbackForce, KnockbackType.FromEnemy);
            BlastArea.gameObject.SetActive(false);
            ShieldAreaObject.SetActive(false);
            
            Laser.gameObject.SetActive(false);
            LaserRain.gameObject.SetActive(false);
            
            agent.enabled = true;
            agent.BlackboardReference.SetVariableValue("IsAlive", true);
            agent.BlackboardReference.SetVariableValue("IsTargetDetected", false);

            ultConditionQueue = new Queue<float>();
            ultConditionQueue.Enqueue(0.75f);
            ultConditionQueue.Enqueue(0.50f);
            ultConditionQueue.Enqueue(0.25f);
        }

        public override void SetTarget(Transform target)
        {
            base.SetTarget(target);
            agent.BlackboardReference.SetVariableValue("IsTargetDetected", target ? true : false);
        }

        public override void OnTakeDamage(IAttacker sender)
        {
            base.OnTakeDamage(sender);
            agent.BlackboardReference.SetVariableValue("Damaged", true);

            DamagedJudge.Kill();
            DamagedJudge = DOVirtual.DelayedCall(0.4f, ()=> agent.BlackboardReference.SetVariableValue("Damaged", false));
        }

        protected override void Die()
        {
            base.Die();
            
            Animator.SetTrigger(EnemyAnimationString.Die);
            agent.BlackboardReference.SetVariableValue("IsAlive", false);
            deathImpact.gameObject.SetActive(true);
            deathImpact.Play();
            Hitbox.enabled = false;
            agent.enabled = false;
        }

        private void OnDestroy()
        {
            ShieldRecharger.Kill();
            TeleportInvincible.Kill();
            DamagedJudge.Kill();
        }
        
        public bool CheckUltCondition()
        {
            if (ultConditionQueue.Count == 0) return false;
            
            if (Stat.CurrentHP / DataSO.HP <= ultConditionQueue.Peek() )
            {
                ultConditionQueue.Dequeue();
                return true;
            }
            return false;
        }
    }
}
