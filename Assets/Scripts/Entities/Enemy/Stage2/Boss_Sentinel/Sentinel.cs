using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ToB.Entities.FieldObject;
using ToB.Entities.Interface;
using ToB.Utils;
using Unity.Behavior;
using UnityEngine;

namespace ToB.Entities
{
    public class Sentinel : Enemy
    {
        public SentinelSO DataSO => enemySO as SentinelSO;
        [field: SerializeField] public EnemyStatHandler Stat { get; private set; }
        [field: SerializeField] public BehaviorGraphAgent Agent { get; private set; }

        [field: SerializeField] public EnemyAttackArea SprintArea { get; private set; }
        [field: SerializeField] public EnemyAttackArea LongSprintArea { get; private set; }

        public Vector2 rangeAttackDirection;
        public Vector2 BodyCenter => Hitbox.bounds.center;

        public float lastRangeAttackTime;
        public float rangeAttackCooldown; // 약/강에 따라 항상 달라짐

        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private GameObject rangeAttackPrefab;
        [SerializeField] private GameObject bloodBubblePrefab;
        public int Phase { get; private set; }
        public Queue<float> SpecialAttackQueue { get; private set; }

        private Tween damagedTween;
        public Coroutine attackCoroutine;

        public bool BubbleAttackEnd { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Agent.BlackboardReference.SetVariableValue("IsAlive", true);
            Agent.BlackboardReference.SetVariableValue("ShieldCooldown", DataSO.BarrierCooldown);
            SpecialAttackQueue = new Queue<float>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Stat.Init(this, DataSO);
            Knockback.Init(this);
            SprintArea.Init(this, DataSO.SprintAttackDamage_1Phase, 15, Vector2.right);
            LongSprintArea.Init(this, DataSO.SprintAttackDamage_1Phase, 15, Vector2.right);
            rangeAttackCooldown = DataSO.RangedAttackPhase1.cooldown;

            Phase = 1;
            
            SpecialAttackQueue.Clear();
            SpecialAttackQueue.Enqueue(0.75f);
            SpecialAttackQueue.Enqueue(0.5f);
            SpecialAttackQueue.Enqueue(0.25f);
            BubbleAttackEnd = false;

            Agent.BlackboardReference.SetVariableValue("JumpSlamChargeTime", 2);
        }

        public override void OnTakeDamage(IAttacker sender)
        {
            base.OnTakeDamage(sender);

            SaveDamagedForJudge();
            PhaseCalculate();
        }

        private void PhaseCalculate()
        {
            if (Phase == 1)
            {
                float currentHP = Stat.CurrentHP - (DataSO.HP - DataSO.HP_1Phase);

                if (currentHP < 0)
                {
                    SetPhase2();
                }
                
                Debug.Log(currentHP / DataSO.HP_1Phase);
                while (SpecialAttackQueue.Count > 0 && SpecialAttackQueue.Peek() > currentHP / DataSO.HP_1Phase)
                {
                    float value = SpecialAttackQueue.Dequeue();

                    if (Mathf.Approximately(value, 0.75f))
                    {
                        Debug.Log("1페이즈 75%");
                        Agent.BlackboardReference.SetVariableValue("CanUseSpecialAttack", true);
                    }
                    else if (Mathf.Approximately(value, 0.5f))
                    {
                        Debug.Log("1페이즈 50%");

                        Agent.BlackboardReference.SetVariableValue("CanUseJumpSlam", true);
                    }
                    else if (Mathf.Approximately(value, 0.25f))
                    {
                        Debug.Log("1페이즈 25%");

                        int num = Random.Range(0, 2);
                        Agent.BlackboardReference.SetVariableValue(num == 0 ? "CanUseSpecialAttack" : "CanUseJumpSlam",
                            true);
                    }
                }
            }
            else if (Phase == 2)
            {
            }
        }

        private void SetPhase2()
        {
            Phase = 2;

            SpecialAttackQueue.Enqueue(0.75f);
            SpecialAttackQueue.Enqueue(0.5f);
            SpecialAttackQueue.Enqueue(0.25f);

            Agent.BlackboardReference.SetVariableValue("JumpSlamChargeTime", 1);

            Stat.ForceSetHP(DataSO.HP - DataSO.HP_1Phase);
        }

        private void SaveDamagedForJudge()
        {
            Agent.BlackboardReference.SetVariableValue("Damaged", true);
            if (damagedTween != null && damagedTween.active) damagedTween.Kill();
            damagedTween =
                DOVirtual.DelayedCall(0.3f, () => Agent.BlackboardReference.SetVariableValue("Damaged", false));
        }

        protected override void Die()
        {
            base.Die();
            Agent.BlackboardReference.SetVariableValue("IsAlive", false);
            if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        }

        public override void SetTarget(Transform target)
        {
            base.SetTarget(target);

            Agent.BlackboardReference.SetVariableValue("IsTargetDetected", target ? true : false);
            Agent.BlackboardReference.SetVariableValue("Target", target);
        }

        #region AttackMethods

        public void RangeAttack()
        {
            Agent.BlackboardReference.GetVariableValue("RangeWeak", out bool isWeak);

            if (isWeak)
            {
                GameObject raObj = rangeAttackPrefab.Pooling();

                raObj.transform.position = BodyCenter + rangeAttackDirection * 0.2f;
                float angle = rangeAttackDirection.ToAngle();

                ContactDamage ra = raObj.GetComponent<ContactDamage>();
                ra.Init(DataSO.RangedAttackPhase1.damage, DataSO.RangedAttackPhase1.knockbackForce, default, false);
                ra.blockable = true;
                ra.effectable = true;
                ra.Position = transform.position;
                ra.transform.eulerAngles = new Vector3(0, 0, angle);

                LinearMovement raMovement = raObj.GetComponent<LinearMovement>();
                raMovement.Init(rangeAttackDirection, DataSO.RangedAttackPhase1.moveSpeed);
            }
        }

        public void StartBubbleAttack()
        {
            BubbleAttackEnd = false;
            Agent.BlackboardReference.SetVariableValue("CanUseSpecialAttack", false);
            attackCoroutine = StartCoroutine(BubbleAttackRoutine());
        }

        IEnumerator BubbleAttackRoutine()
        {
            for (int i = 0; i < 5; i++)
            {
                SpawnBloodBubble();
                yield return new WaitForSeconds(1.2f);
            }

            BubbleAttackEnd = true;
        }

        private void SpawnBloodBubble()
        {
            RaycastHit2D hit = Physics2D.Raycast(target.transform.position, Vector2.down, 100, groundLayer);
            Vector2 spawnPos = hit.point;
            
            GameObject blood = bloodBubblePrefab.Pooling();
            blood.transform.position = spawnPos;
        }

        #endregion
    }
}