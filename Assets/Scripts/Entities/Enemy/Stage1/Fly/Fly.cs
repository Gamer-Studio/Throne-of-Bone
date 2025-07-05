


using System.Collections;
using UnityEngine;

namespace ToB.Entities
{
    public class Fly : Enemy
    {
        public FlySO DataSO { get; private set; }
        [field:SerializeField] public Hive Hive { get; private set; }
        
        [field:SerializeField] public FlyFSM FSM { get; private set; }
        [field:SerializeField] public EnemyBody Body { get; private set; }
        [field:SerializeField] public EnemyRangeBaseSightSensor RangeBaseSightSensor { get; private set; }
        [field:SerializeField] public EnemyStatHandler Stat { get; private set; }
        
        [field:SerializeField] public GameObject StingPrefab;

        public bool IsInPatrolArea =>
            (Hive.PatrolRange.gameObject.transform.position - transform.position).sqrMagnitude <
            Mathf.Pow(Hive.DataSO.PatrolRange, 2);

        public bool TargetInAttackRange => (target.position - transform.position).sqrMagnitude < Mathf.Pow(DataSO.AttackRange, 2);

        protected override void Awake()
        {
            base.Awake();
            Body.Init(this, DataSO.ATK);
            RangeBaseSightSensor.Init(this);
            Knockback = GetComponentInChildren<EnemyKnockback>();
            Knockback.Init(this);
            DataSO = enemySO as FlySO;
        }

        public void Init(Hive hive)
        {
            Hive = hive;
            Stat.Init(this, DataSO);
            isAlive = true;
            Hitbox.enabled = true;
            FSM.Init();
        }

        protected override void Die()
        {
            base.Die();
            Animator.SetTrigger(EnemyAnimationString.Die);
            Hive.flies.Remove(gameObject);
            Hitbox.enabled = false;
            StartCoroutine(ReleaseSelf());
        }

        IEnumerator ReleaseSelf()
        {
            yield return new WaitForSeconds(2f);
            gameObject.Release();
        }

        public void StingAttack()
        {
            GameObject stingObj = StingPrefab.Pooling();
            LinearProjectile sting = stingObj.GetComponent<LinearProjectile>();
            Vector2 direction = GetTargetDirection();
            
            stingObj.transform.position = transform.position;
            sting.LinearMovement.Init(direction, DataSO.StingSpeed);
            sting.ContactDamage.Init(DataSO, direction);
        }
    }
}
