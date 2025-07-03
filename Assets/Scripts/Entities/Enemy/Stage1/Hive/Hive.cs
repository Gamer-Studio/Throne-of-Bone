using System;
using System.Collections.Generic;
using ToB.Entities;
using UnityEngine;

namespace ToB
{
    public class Hive : Enemy
    {
        [field:SerializeField] HiveSO Data;
        [field:SerializeField] GameObject flyPrefab;
        [field: SerializeField] public List<GameObject> flies;

        [field: SerializeField] private EnemyStatHandler stat;
        
        [field: SerializeField] public CircleLocation PatrolRange { get; private set; }
        [field: SerializeField] public CircleLocation ChaseRange { get; private set; }
        

        private float lastSummonTime;

        protected override void Awake()
        {
            base.Awake();
            
            flies = new List<GameObject>();
            stat.Init(this, Data.HP);
            PatrolRange.Init(Data.PatrolRange);
            ChaseRange.Init(Data.ChaseRange);
        }

        protected override void Reset()
        {
            base.Reset();
            stat = GetComponentInChildren<EnemyStatHandler>();
        }

        private void Update()
        {
            lastSummonTime += Time.deltaTime;

            if (lastSummonTime > Data.FlyRegenTimeInterval && flies.Count < Data.FlyRegenAmount)
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
            
            flyObj.transform.SetParent(transform);
            flies.Add(flyObj);

            Fly newFly = flyObj.GetComponent<Fly>();
            newFly.Init(this);
        }


        private void OnDestroy()
        {
            foreach (var fly in flies)
            {
                if(fly) fly.Release();
            }
        }
    }
}
