using UnityEngine;

namespace ToB.Entities
{
    public class MutantRat : 
        Enemy, 
        IEnemyHitPart
    {
        
        [field:SerializeField] public MutantRatSO DataSO { get; private set; }
        [field:SerializeField] public EnemyStatHandler Stat { get; private set; }
        [field:SerializeField] public EnemySightSensor SightSensor { get; private set; }
        [field:SerializeField] public MutantRatFSM FSM { get; private set; }
        
        [field:SerializeField] public ParticleSystem deathEffect { get; private set; }
        

        protected override void Awake()
        {
            base.Awake();
            Stat.Init(this, DataSO.HP, 0);
            Knockback.Init(this, DataSO.KnockbackMultiplier);
            SightSensor.Init(this, DataSO.SightRange, DataSO.SightAngle);
            FSM.Init();
            deathEffect.gameObject.SetActive(false);
        }
        protected override void Reset()
        {
            Stat = GetComponentInChildren<EnemyStatHandler>();
            FSM = GetComponent<MutantRatFSM>();
        }

        protected override void Die()
        {
            base.Die();
            
            deathEffect.gameObject.SetActive(true);
            deathEffect.transform.SetParent(null);
            deathEffect.Play();
            
            Destroy(gameObject);
            Destroy(deathEffect,2);
        }
    }
}
