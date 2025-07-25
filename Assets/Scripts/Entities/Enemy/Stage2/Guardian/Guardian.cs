using System;
using DG.Tweening;
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

        [SerializeField] private BehaviorGraphAgent agent;

        [SerializeField] private ParticleSystem deathImpact;

        public Tween ShieldRecharger;

        protected override void Awake()
        {
            base.Awake();
            agent.BlackboardReference.SetVariableValue("ShieldCooldown", DataSO.ShieldRechargeTime);
            agent.BlackboardReference.SetVariableValue("TeleportCooldown", DataSO.TeleportRechargeTime);
            agent.BlackboardReference.SetVariableValue("BlastCooldown", DataSO.BlastRechargeTime);
            agent.BlackboardReference.SetVariableValue("LaserCooldown", DataSO.LaserRechargeTime);
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
            
            
            agent.enabled = true;
            agent.BlackboardReference.SetVariableValue("IsAlive", true);
            agent.BlackboardReference.SetVariableValue("IsTargetDetected", false);
        }

        public override void SetTarget(Transform target)
        {
            base.SetTarget(target);
            agent.BlackboardReference.SetVariableValue("IsTargetDetected", target ? true : false);
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
        }
    }
}
