using UnityEngine;

namespace ToB.Entities
{
    [CreateAssetMenu(fileName = "SentinelSO", menuName = "Scriptable Objects/Enemy/Stage2/SentinelSO")]
    public class SentinelSO : EnemySO
        , IEnemyMainBodySO
        , IEnemyGroundMoveSO
        , IEnemyHittableSO
        , IEnemyKnockbackSO
    {
        [field: SerializeField] public float BodyDamage { get; private set; } = 10;
        [field: SerializeField] public float MoveSpeed { get; private set; } = 3;
        [field: SerializeField] public float HP { get; private set; } = 800;
        [field: SerializeField] public float HP_1Phase { get; private set; } = 300;
        [field: SerializeField] public float DEF { get; private set; }
        [field: SerializeField] public float KnockbackMultiplier { get; private set; } = 0.8f;

        [field: SerializeField] public float SprintAttackDamage_1Phase { get; private set; } = 10;
        [field: SerializeField] public float SprintAttackDamage_2Phase { get; private set; } = 15;
        [field: SerializeField] public float SprintAttackCooldown { get; private set; } = 5;
        
        [field: SerializeField] public float SlashWaveDamage { get; private set; } = 20;
        [field: SerializeField] public float SlashWaveChargeTime_1Phase { get; private set; } = 2;
        [field: SerializeField] public float SlashWaveChargeTime_2Phase { get; private set; } = 1;
        
        [field: SerializeField] public float BarrierCooldown { get; private set; } = 8;
        [field: SerializeField] public float BarrierDuration { get; private set; } = 2;

        [field: SerializeField] public SentinelRangedAttack RangedAttackPhase1 { get; private set; } =
            new SentinelRangedAttack() { chargeTime = 1, cooldown = 3,  damage = 10, knockbackForce = 3, moveSpeed = 25};
        [field: SerializeField] public SentinelRangedAttack RangedAttackPhase2 { get; private set; } =
            new SentinelRangedAttack() { chargeTime = 2, cooldown = 5,  damage = 15, knockbackForce = 2.5f, moveSpeed = 25};

        [field: SerializeField] public float BloodBubbleInterval { get; private set; } = 1.2f;
        [field: SerializeField] public int BloodBubbleCycle { get; private set; } = 5;
        
        [field: SerializeField] public float BloodBubblesInterval { get; private set; } = 1.8f;
        [field: SerializeField] public int BloodBubblesCycle { get; private set; } = 2;
    }

    [System.Serializable]
    public struct SentinelRangedAttack
    {
        public float chargeTime;
        public float cooldown;
        public float damage;
        public float knockbackForce;
        public float moveSpeed;
    }
}