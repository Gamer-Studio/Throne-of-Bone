


using System;
using System.Collections;
using ToB.Entities.FieldObject;
using ToB.Entities.Interface;
using ToB.Utils;
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
            DataSO = enemySO as FlySO;
            
            if(!DataSO) Debug.Log("SO 가 없습니다 (Fly)");
            
            Body.Init(this, DataSO.ATK);
            RangeBaseSightSensor.Init(this);
            Knockback = GetComponentInChildren<EnemyKnockback>();
            Knockback.Init(this);
        }

        private void Update()
        {
            if (target && Hive.RangeBaseSightSensor.TargetInRange)
            {
                Hive.target = target;
            }
        }


        public void Init(Hive hive)
        {
            Hive = hive;
            Stat.Init(this, DataSO);
            isAlive = true;
            Hitbox.enabled = true;
            audioPlayer.Play("Organism_15");
            FSM.Init();
        }

        public override void OnTakeDamage(IAttacker sender)
        {
            base.OnTakeDamage(sender);
            if (Stat.CurrentHP > 0) audioPlayer.Play("Insect_Hurt_04");
        }

        protected override void Die()
        {
            base.Die();
            Animator.SetTrigger(EnemyAnimationString.Die);
            Hive.flies.Remove(gameObject);
            Hitbox.enabled = false;
            audioPlayer.Play("Insect_Death_04");
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
            LinearMovement linearMovement = stingObj.GetComponent<LinearMovement>();
            ContactDamage contactDamage = stingObj.GetComponent<ContactDamage>();
            Vector2 direction = GetTargetDirection();
            
            stingObj.transform.position = transform.position;
            linearMovement.Init(direction, DataSO.StingSpeed);
            contactDamage.Init(DataSO, direction);
        }
    }
}
