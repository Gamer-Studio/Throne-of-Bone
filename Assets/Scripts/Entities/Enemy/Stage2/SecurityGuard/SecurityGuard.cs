using System.Collections.Generic;
using ToB.Entities.Interface;
using UnityEngine;

namespace ToB.Entities
{
    public class SecurityGuard : Enemy
    {
        public SecurityGuardSO DataSO => enemySO as SecurityGuardSO;

        [Header("고유 컴포넌트")] [field: SerializeField]
        public EnemyRangeBaseSightSensor SightSensor { get; private set; }

        [field: SerializeField] public EnemyStatHandler Stat { get; private set; }
        [field: SerializeField] public EnemyBody EnemyBody { get; private set; }
        [field: SerializeField] public SecurityGuardFSM FSM { get; private set; }
        [field: SerializeField] public EnemySimpleSensor AttackSensor { get; private set; }
        [field: SerializeField] public List<EnemyAttackArea> AttackAreas { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();

            Stat.Init(this, DataSO);
            EnemyBody.Init(this, DataSO.BodyDamage);
            SightSensor.Init(this);
            Knockback.Init(this);
            AttackAreas.ForEach(area =>
            {
                area.Init(this, DataSO.AttackDamage, DataSO.AttackKnockbackForce, Vector2.right);
                area.gameObject.SetActive(false);
            });
            FSM.Init();
        }

        public override void OnTakeDamage(IAttacker sender)
        {
            base.OnTakeDamage(sender);
            audioPlayer.Play("Orc_Hurt_02");
        }
        

        protected override void Die()
        {
            base.Die();
            audioPlayer.StopAll();
            audioPlayer.Play("Orc_Death_05");
            Animator.SetTrigger(EnemyAnimationString.Die);
            Hitbox.enabled = false;
            FSM.ChangePattern(null);
        }
    }
}