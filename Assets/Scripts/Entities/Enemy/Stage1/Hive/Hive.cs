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

        private float lastSummonTime;

        protected override void Awake()
        {
            base.Awake();
            
            flies = new List<GameObject>();
        }

        private void Update()
        {
            lastSummonTime += Time.deltaTime;
            
        }
    }
}
