using ToB.Entities.Interface;
using UnityEngine;

namespace ToB.Entities
{
    public class SecurityArcher : Enemy, IEnemyDefault
    {
        public SecurityArcherSO DataSO => EnemySO as SecurityArcherSO;
        [field:SerializeField] public EnemyBody EnemyBody { get; private set; }
        [field:SerializeField] public EnemyStatHandler Stat { get; private set; }
        [field:SerializeField]public EnemyRangeBaseSightSensor SightSensor { get; private set; }
        [field:SerializeField] public SecurityArcherFSM FSM { get; private set; }
        
        [field:SerializeField] public EnemySimpleSensor NormalAttackSensor { get; private set; }
        [field:SerializeField] public EnemySimpleSensor DodgeSensor { get; private set; }
        [field:SerializeField] public ParticleSystem DodgeEffect { get; private set; }
        [field:SerializeField] public EnemyBeamAttack BeamAttack { get; private set; }

        public Vector2 dodgeEffectOffset;

        protected override void Awake()
        {
            base.Awake();
            dodgeEffectOffset = DodgeEffect.transform.localPosition;
        }

        public override void OnTakeDamage(IAttacker sender)
        {
            base.OnTakeDamage(sender);
            audioPlayer.Play("Spectre_Hurt_02");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            EnemyBody.Init(this, DataSO.BodyDamage);
            Stat.Init(this, DataSO);
            FSM.Init();
            SightSensor.Init(this);
            Knockback.Init(this);
            BeamAttack.Init(DataSO.BeamTickDamage, DataSO.BeamTickDamage * 4);
        }

        protected override void Die()
        {
            base.Die();
            audioPlayer.StopAll();
            audioPlayer.Play("Spectre_Death_02");
            Hitbox.enabled = false;
            FSM.ChangePattern(null);
            Animator.SetTrigger(EnemyAnimationString.Die);
        }
    }
}
