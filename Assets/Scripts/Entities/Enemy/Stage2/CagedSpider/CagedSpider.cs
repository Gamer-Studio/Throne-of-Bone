using System.Collections;
using ToB.Entities.Interface;
using UnityEngine;

namespace ToB.Entities
{
    public class CagedSpider : Enemy
    {
        CagedSpiderSO DataSO => EnemySO as CagedSpiderSO;
        
        [Header("고유 컴포넌트")] 
        [SerializeField] private CagedSpiderFSM fsm;
        public CagedSpiderFSM FSM => fsm;
        [field:SerializeField] public EnemyRangeBaseSightSensor SightSensor { get; private set; }
        [field:SerializeField] public EnemyStatHandler Stat { get; private set; }

        [SerializeField] private EnemyAttackArea explodeArea; 
        [SerializeField] private CircleCollider2D explodeAreaCollider; 
        [field:SerializeField] public EnemyBody EnemyBody { get; private set; }
        [SerializeField] private ParticleSystem explodeEffect;        

        protected override void OnEnable()
        {
            base.OnEnable();
            
            FSM.Init();
            SightSensor.Init(this);
            Stat.Init(this, EnemySO as IEnemyHittableSO);
            Knockback.Init(this);
            EnemyBody.Init(this, DataSO.BodyDamage);
            
            explodeArea.Init(this, DataSO.ExplodeDamage, DataSO.ExplodePower);
            explodeAreaCollider.radius = DataSO.ExplodeRadius;
            explodeArea.gameObject.SetActive(false);
            explodeEffect.gameObject.SetActive(false);
        }

        public override void OnTakeDamage(IAttacker sender)
        {
            base.OnTakeDamage(sender);
            audioPlayer.Play("Spider_Hurt_03");
        }

        protected override void Die()
        {
            base.Die();
            audioPlayer.StopAll();
            Animator.SetTrigger(EnemyAnimationString.Die);
            Hitbox.enabled = false;
            explodeArea.gameObject.SetActive(true);
            StartCoroutine(ExplodeEnd());
            FSM.ChangePattern(null);
            
            explodeEffect.gameObject.SetActive(true);
            explodeEffect.Play();
        }

        IEnumerator ExplodeEnd()
        {
            yield return new WaitForSeconds(0.1f);
            explodeArea.gameObject.SetActive(false);
        }
    }
}
