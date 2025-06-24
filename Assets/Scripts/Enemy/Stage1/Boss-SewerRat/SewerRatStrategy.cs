using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ToB
{
    public class SewerRatStrategy : EnemyStrategy
    {
        SewerRatDigPattern digPattern;
        SewerRatScratchPattern scratchPattern;
        SewerRatToxicBonePattern toxicBonePattern;

        [Header("패턴 도우미")]
        [SerializeField] private float patternDistance;
        private bool OutOfMeleeRange => enemy.GetTargetDistanceSQR() > Mathf.Pow(patternDistance, 2);

        [SerializeField] private float breathTime = 1;
        private float coolDown;

        public Location ascendLocation;
        [field:SerializeField] public LayerMask GroundLayer { get; private set; }
        [field:SerializeField] public GameObject toxicBonePrefab { get; private set; }
        
        [Header("현재 패턴")]
        // 아래 string 값은 관측용으로 씁니다.
        // Alt+Enter에서 지원하는 기능으로 경고 무효 처리 했습니다.
#pragma warning disable CS0414 
        [SerializeField] private string currentPatternName;
#pragma warning restore CS0414 
        
        protected override void Awake()
        {
            base.Awake();
            digPattern = new SewerRatDigPattern(enemy, this, PatternEnd);
            scratchPattern = new SewerRatScratchPattern(enemy, this, PatternEnd);
            toxicBonePattern = new SewerRatToxicBonePattern(enemy, this, PatternEnd);
        }

        public override void Init()
        {
            enemy.Physics.collisionEnabled = true;
            enemy.Physics.gravityEnabled = true;
            
            coolDown = breathTime;
            currentPatternName = "";
        }

        private void Update()
        {
            
            if (currentPattern != null)
            {
                currentPattern.Execute();
            }
            else if (!enemy.IsGrounded)
            {
                return;
            }
            else if (coolDown > 0) coolDown -= Time.deltaTime;
            else ChooseNextPattern();
        }

        private void ChooseNextPattern()
        {
            float rand = Random.value;

            float[] patternSection = new float[2];
            
            if (OutOfMeleeRange)        
            {
                patternSection[0] = 0.5f;
                patternSection[1] = 0.75f;
            }
            else
            {
                patternSection[0] = 0.25f;
                patternSection[1] = 0.5f;
            }
            
            if (rand < patternSection[0])
            {
                currentPatternName = "Scratch";
                TriggerPattern(scratchPattern);
            }
            else if (rand < patternSection[1])
            {
                currentPatternName = "Dig";
                TriggerPattern(digPattern);
            }
            else
            {
                currentPatternName = "Toxic Bone";
               TriggerPattern(toxicBonePattern);
            }
        }

        private void TriggerPattern(EnemyPattern pattern)
        {
            currentPattern = pattern;
            currentPattern.Enter();
            coolDown = breathTime;
        }

        private void PatternEnd()
        {
            currentPattern = null;   
            currentPatternName = "Breath";
        }
    }
}
