using ToB.Entities.FieldObject;
using ToB.Utils;
using Unity.Behavior;
using UnityEngine;

namespace ToB.Entities
{
    public class Sentinel : Enemy
    {
        public SentinelSO DataSO => enemySO as SentinelSO;
        [field:SerializeField] public EnemyStatHandler Stat { get; private set; }
        [field:SerializeField] public BehaviorGraphAgent Agent { get; private set; }

        [SerializeField] private GameObject rangeAttackPrefab;
        public Vector2 rangeAttackDirection;
        public Vector2 BodyCenter => Hitbox.bounds.center;

        public float lastRangeAttackTime;
        public float rangeAttackCooldown;  // 약/강에 따라 항상 달라짐
        protected override void Awake()
        {
            base.Awake();
            Agent.BlackboardReference.SetVariableValue("IsAlive", true);
            
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Stat.Init(this, DataSO);
            Knockback.Init(this);
        }

        protected override void Die()
        {
            base.Die();
            Agent.BlackboardReference.SetVariableValue("IsAlive", false);
        }
        public override void SetTarget(Transform target)
        {
            base.SetTarget(target);

            Agent.BlackboardReference.SetVariableValue("IsTargetDetected", target ? true : false);
            Agent.BlackboardReference.SetVariableValue("Target", target );
        }

        public void RangeAttack()
        {
            Agent.BlackboardReference.GetVariableValue("RangeWeak", out bool isWeak);

            if (isWeak)
            {
                GameObject raObj = rangeAttackPrefab.Pooling();
                
                raObj.transform.position = BodyCenter + rangeAttackDirection * 0.2f;
                float angle = rangeAttackDirection.ToAngle();
                
                ContactDamage ra = raObj.GetComponent<ContactDamage>();
                ra.Init(DataSO.RangedAttackPhase1.damage, DataSO.RangedAttackPhase1.knockbackForce,default, false);
                ra.Blockable = true;
                ra.Effectable = true;
                ra.Position = transform.position;
                ra.transform.eulerAngles = new Vector3(0, 0, angle);
                
                LinearMovement raMovement = raObj.GetComponent<LinearMovement>();
                raMovement.Init(rangeAttackDirection, DataSO.RangedAttackPhase1.moveSpeed);
            }
            
        }
    }
}
