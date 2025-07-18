
using UnityEngine;

namespace ToB.Entities
{
    public class SecurityGuard : Enemy
    {
        public SecurityGuardSO DataSO => enemySO as SecurityGuardSO;
        
        [Header("고유 컴포넌트")] 
        [field:SerializeField] public EnemyRangeBaseSightSensor SightSensor { get; private set; }
        [field:SerializeField] public EnemyStatHandler Stat { get; private set; }
        [field:SerializeField] public EnemyBody EnemyBody { get; private set; }
        [field:SerializeField] public SecurityGuardFSM FSM { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            Stat.Init(this, DataSO);
            EnemyBody.Init(this, DataSO.BodyDamage);
            SightSensor.Init(this);
            Knockback.Init(this);
            FSM.Init();
        }
    }
}
