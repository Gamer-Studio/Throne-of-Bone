using UnityEngine;

namespace ToB.Entities
{
    public class CagedSpider : Enemy
    {
        [Header("고유 컴포넌트")] 
        [SerializeField] private CagedSpiderFSM fsm;
        public CagedSpiderFSM FSM => fsm;
        [field:SerializeField] public EnemyRangeBaseSightSensor SightSensor { get; private set; }
        [field:SerializeField] public EnemyStatHandler Stat { get; private set; }
        

        protected override void OnEnable()
        {
            base.OnEnable();
            
            FSM.Init();
            SightSensor.Init(this);
            Stat.Init(this, EnemySO as IEnemyHittableSO);
        }
    }
}
