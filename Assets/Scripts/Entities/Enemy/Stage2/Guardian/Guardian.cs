using System;
using DG.Tweening;
using Unity.Behavior;
using UnityEngine;

namespace ToB.Entities
{
    public class Guardian : Enemy
    {
        public GuardianSO GuardianSO => enemySO as GuardianSO;
        [field:SerializeField] public EnemyBody EnemyBody { get; private set; }
        [field:SerializeField] public EnemyStatHandler Stat { get; private set; }
        [field:SerializeField] public GameObject ShieldAreaObject { get; private set; }
        [field:SerializeField] public EnemySimpleSensor AttackableAreaInnerSensor { get; private set; }
        [field:SerializeField] public EnemySimpleSensor AttackableAreaOuterSensor { get; private set; }

        [SerializeField] private BehaviorGraphAgent agent;

        [SerializeField] private ParticleSystem deathImpact;

        public Tween ShieldRecharger;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            EnemyBody.Init(this, GuardianSO.BodyDamage);
            Stat.Init(this, GuardianSO);
            Knockback.Init(this);
            ShieldAreaObject.SetActive(false);
            agent.enabled = true;
            agent.BlackboardReference.SetVariableValue("IsAlive", true);

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
