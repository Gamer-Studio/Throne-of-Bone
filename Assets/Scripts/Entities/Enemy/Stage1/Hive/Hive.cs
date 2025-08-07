using System.Collections.Generic;
using NaughtyAttributes;
using ToB.Entities;
using ToB.Utils;
using UnityEngine;

namespace ToB
{
    public class Hive : Enemy
    {
        public HiveSO DataSO { get; private set; }
        [field: SerializeField] GameObject flyPrefab;
        [field: SerializeField] public List<GameObject> flies;

        [field: SerializeField] private EnemyStatHandler stat;
        [field: SerializeField] public CircleLocation PatrolRange { get; private set; }
        [field: SerializeField] public CircleLocation ChaseRange { get; private set; }
        [field: SerializeField] public EnemyRangeBaseSightSensor RangeBaseSightSensor { get; private set; }

        private float lastSummonTime;

        protected override void Awake()
        {
            base.Awake();
            DataSO = enemySO as HiveSO;
            
            if(!DataSO) Debug.LogError("HiveSO가 없습니다");
            
            
            
            PatrolRange.Init(DataSO.PatrolRange);
            ChaseRange.Init(DataSO.ChaseRange);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            flies = new List<GameObject>();
            RangeBaseSightSensor.Init(this);
            
            stat.Init(this, DataSO);
        }

        protected override void Reset()
        {
            base.Reset();
            stat = GetComponentInChildren<EnemyStatHandler>();
        }

        private void Update()
        {
            lastSummonTime += Time.deltaTime;

            if (lastSummonTime > DataSO.FlyRegenTimeInterval && flies.Count < DataSO.FlyRegenAmount)
            {
                SummonFly();
            }
        }

        private void SummonFly()
        {
            lastSummonTime = 0;

            GameObject flyObj = flyPrefab.Pooling();

            flyObj.transform.position = PatrolRange.GetRandomPosition();

            if (flyObj.transform.position.y > transform.position.y)
            {
                Vector3 pos = flyObj.transform.position;
                pos.y = transform.position.y;
                flyObj.transform.position = pos;
            }

            flies.Add(flyObj);

            Fly newFly = flyObj.GetComponent<Fly>();
            newFly.Init(this);
        }

        protected override void Die()
        {
            base.Die();
            Animator.SetTrigger(EnemyAnimationString.Die);
            Hitbox.enabled = false;
            enabled = false;
            audioPlayer.Play("hit_blood_flesh_gore_03");
        }

        public bool IsFlyInPatrolArea(Fly fly)
        {
            return (fly.transform.position - PatrolRange.transform.position).sqrMagnitude < Mathf.Pow(DataSO.PatrolRange, 2);
        }
        // private void OnDestroy()
        // {
        //     foreach (var fly in flies)
        //     {
        //         if (fly) fly.Release();
        //     }
        // }
    }
}