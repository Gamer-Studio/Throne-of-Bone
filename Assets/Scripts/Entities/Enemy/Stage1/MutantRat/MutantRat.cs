using ToB.Entities.Interface;
using UnityEngine;

namespace ToB.Entities
{
    public class MutantRat : 
        Enemy, 
        IEnemyHitPart
    {
        
        public MutantRatSO DataSO { get; private set; }
        [field:SerializeField] public EnemyStatHandler Stat { get; private set; }
        [field:SerializeField] public EnemyRangeBaseSightSensor RangeBaseSightSensor { get; private set; }
        [field:SerializeField] public EnemySimpleSensor AttackSensor { get; private set; }
        [field:SerializeField] public MutantRatFSM FSM { get; private set; }
        
        [field:SerializeField] public ParticleSystem deathEffect { get; private set; }
        

        protected override void Awake()
        {
            base.Awake();
            DataSO = enemySO as MutantRatSO;
            
            Knockback.Init(this);
            RangeBaseSightSensor.Init(this);
            FSM.Init();
            deathEffect.gameObject.SetActive(false);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Stat.Init(this, DataSO);
            
        }

        protected override void Reset()
        {
            Stat = GetComponentInChildren<EnemyStatHandler>();
            FSM = GetComponent<MutantRatFSM>();
        }

        public override void OnTakeDamage(IAttacker sender)
        {
            base.OnTakeDamage(sender);
            if (Stat.CurrentHP > 0) audioPlayer.Play("Scream_01");
        }

        protected override void Die()
        {
            base.Die();
            
            deathEffect.gameObject.SetActive(true);
            deathEffect.transform.SetParent(null);
            deathEffect.Play();
            audioPlayer.Play("Death_01");
            
            Destroy(gameObject);
            Destroy(deathEffect,2);
        }
    }
}
