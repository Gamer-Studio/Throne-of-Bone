using System;
using Unity.Collections;
using UnityEngine;

namespace ToB
{
    public abstract class EnemyStrategy : MonoBehaviour
    {
        [field:SerializeField, ReadOnly] public Enemy enemy { get; private set; }
        protected EnemyPattern currentPattern { get; set; }

        protected virtual void Awake()
        {
            enemy = GetComponent<Enemy>();
        }

        public abstract void Init();


    }
}
