using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ToB.Entities.FieldObject;
using ToB.Entities.Interface;
using ToB.Utils;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.VFX;

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

        [Header("영역 컴포넌트")] 
        [SerializeField] private SentinelArea area;
       
        [Header("점프 슬램")]
        [SerializeField] private GameObject shockwavePrefab;
        [SerializeField] private VisualEffect shockwaveEffect;
        [SerializeField] private Transform shockwaveSpawnPositionLeft;
        [SerializeField] private Transform shockwaveSpawnPositionRight;
        [SerializeField] private float shockwaveAccelPower = 3;

        [Header("2페이즈")] 
        [SerializeField] private VisualEffect phase2Aura;
        public int Phase { get; private set; }
        public Queue<float> SpecialAttackQueue { get; private set; }

        private Tween damagedTween;
        public Coroutine attackCoroutine;

        public bool BubbleAttackEnd { get; private set; }

        [Header("클론 전용 필드")] 
        public bool isClone;
        public Sentinel original;
        public float prevHP;

        protected override void Awake()
        {
            base.Awake();
            Agent.BlackboardReference.SetVariableValue("IsAlive", true);
            Agent.BlackboardReference.SetVariableValue("ShieldCooldown", DataSO.BarrierCooldown);
            SpecialAttackQueue = new Queue<float>();
            shockwaveEffect.Stop();
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
            phase2Aura.gameObject.SetActive(false);

            if (!isClone)
            {
                SpecialAttackQueue.Clear();
                SpecialAttackQueue.Enqueue(0.75f);
                SpecialAttackQueue.Enqueue(0.5f);
                SpecialAttackQueue.Enqueue(0.25f);
            }

            BubbleAttackEnd = false;
            prevHP = Stat.CurrentHP;

            Agent.BlackboardReference.SetVariableValue("JumpSlamChargeTime", 2f);
            Agent.BlackboardReference.SetVariableValue("Phase", 1);
        }

        public override void OnTakeDamage(IAttacker sender)
        {
            base.OnTakeDamage(sender);

            if (!isAlive) return;
            
            SaveDamagedForJudge();
            
            if(!isClone) PhaseCalculate();
            else
            {
                float deltaHP = prevHP - Stat.CurrentHP;
                original.Stat.ChangeHP(-deltaHP);
                prevHP = Stat.CurrentHP;
            }
        }

        private void PhaseCalculate()
        {
            if (Phase == 1)
            {
                float currentHP = Stat.CurrentHP - (DataSO.HP - DataSO.HP_1Phase);

                if (currentHP < 0)
                {
                    StartCoroutine(SetPhase2());
                    return;
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
                float currentHP = Stat.CurrentHP;
                Debug.Log("HP perc :" + currentHP / (DataSO.HP - DataSO.HP_1Phase));

                while (SpecialAttackQueue.Count > 0 && SpecialAttackQueue.Peek() > currentHP / (DataSO.HP - DataSO.HP_1Phase))    // 체력 캐싱은 리팩토링할 때 이 클래스의 필드에
                {
                    float value = SpecialAttackQueue.Dequeue();

                    if (Mathf.Approximately(value, 0.8f))
                    {
                        Debug.Log("2페이즈 80%");
                        Agent.BlackboardReference.SetVariableValue("CanUseSpecialAttack", true);
                    }
                    else if (Mathf.Approximately(value, 0.6f))
                    {
                        Debug.Log("2페이즈 60%");

                        Agent.BlackboardReference.SetVariableValue("CanUseClone", true);
                    }
                    else if (Mathf.Approximately(value, 0.2f))
                    {
                        Debug.Log("2페이즈 20%");
                        
                        Agent.BlackboardReference.SetVariableValue("CanUseSpecialAttack", true);
                    }
                }
            }
        }

        IEnumerator SetPhase2()
        {
            Agent.End();
            Agent.BlackboardReference.SetVariableValue("SentinelState", SentinelState.Idle);

            Phase = 2;
            Agent.BlackboardReference.SetVariableValue("Phase", 2);

            SpecialAttackQueue.Clear();
            SpecialAttackQueue.Enqueue(0.8f);
            SpecialAttackQueue.Enqueue(0.6f);
            SpecialAttackQueue.Enqueue(0.2f);

            Agent.BlackboardReference.SetVariableValue("JumpSlamChargeTime", 1f);

            Stat.ForceSetHP(DataSO.HP - DataSO.HP_1Phase);
            
            
            Animator.SetTrigger(EnemyAnimationString.Idle);
            
            Hitbox.enabled = false;

            yield return new WaitUntil(() => IsGrounded);
            
            shockwaveEffect.SendEvent("OnPlay");
            phase2Aura.gameObject.SetActive(true);
            phase2Aura.Play();
            
            yield return new WaitForSeconds(1.5f);
            
            Hitbox.enabled = true;
            Agent.Init();
            Agent.Restart();
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
                // Weak과 Strong 거의 중복이지만 작성 편의상 복사했습니다
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
            else
            {
                GameObject raObj = rangeAttackPrefab.Pooling();

                raObj.transform.position = BodyCenter + rangeAttackDirection * 0.2f;
                float angle = rangeAttackDirection.ToAngle();
                raObj.transform.localScale = Vector3.one * 1.15f;

                ContactDamage ra = raObj.GetComponent<ContactDamage>();
                ra.Init(DataSO.RangedAttackPhase2.damage, DataSO.RangedAttackPhase2.knockbackForce, default, false);
                ra.blockable = true;
                ra.effectable = true;
                ra.Position = transform.position;
                ra.transform.eulerAngles = new Vector3(0, 0, angle);

                LinearMovement raMovement = raObj.GetComponent<LinearMovement>();
                raMovement.Init(rangeAttackDirection, DataSO.RangedAttackPhase2.moveSpeed);
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
            Vector2 origin = target.transform.position;
            Vector2 direction = Vector2.down;
            float distance = 100f;
            Debug.DrawRay(origin, direction * distance, Color.cyan, 1.0f); // 마지막 인자는 지속 시간(1초)
            
            RaycastHit2D hit = Physics2D.Raycast(target.transform.position, Vector2.down, 100, groundLayer);
            Vector2 spawnPos = hit.point;
            
            GameObject blood = bloodBubblePrefab.Pooling();
            blood.transform.position = spawnPos;
        }

        public void GenerateShockWave()
        {
            shockwaveEffect.SendEvent("OnPlay");
            StartCoroutine(GenerateShockWaveCoroutine());
            
        }

        IEnumerator GenerateShockWaveCoroutine()
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject waveObj = shockwavePrefab.Pooling();
                waveObj.transform.position = shockwaveSpawnPositionRight.position;
                waveObj.transform.localScale = LookDirectionHorizontal + new Vector2(0,1);
                
                AccelerateMovement waveAccel = waveObj.GetComponent<AccelerateMovement>();
                waveAccel.SetAcceleration(LookDirectionHorizontal * (shockwaveAccelPower));
                
                waveObj = shockwavePrefab.Pooling();
                waveObj.transform.position = shockwaveSpawnPositionLeft.position;
                waveObj.transform.localScale = -LookDirectionHorizontal + new Vector2(0, 1);
                
                waveAccel = waveObj.GetComponent<AccelerateMovement>();
                waveAccel.SetAcceleration(LookDirectionHorizontal * (-shockwaveAccelPower));
                
                yield return new WaitForSeconds(0.02f);
            }
        }
        
        #endregion


        public void ClonePattern()
        {
            Hitbox.enabled = false;
            StartCoroutine(SpawnClones());
        }

        IEnumerator SpawnClones()
        {
            yield return new WaitForSeconds(1.5f);
            area.SpawnClones();
        }

        public void ClonePatternDisable()
        {
            area.ClearClones();
            DOVirtual.DelayedCall(1.5f, () =>
            {
                Hitbox.enabled = true;
            });
        }
    }
}