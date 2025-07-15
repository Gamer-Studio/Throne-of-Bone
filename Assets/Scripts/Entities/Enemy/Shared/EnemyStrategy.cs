using System;
using Unity.Collections;
using UnityEngine;

namespace ToB.Entities
{
    // fsm과 strategy 뼈대가 같아서 변수명으로 설계 의도를 표현하는 것으로 합니다
    public abstract class EnemyStrategy : MonoBehaviour
    {
        [field:SerializeField, ReadOnly] public Enemy enemy { get; private set; }
        protected EnemyPattern currentPattern { get; set; }
        [SerializeField] string currentPatternString;
        
        protected virtual void Awake()
        {
            enemy = GetComponent<Enemy>();
        }

        private void Reset()
        {
            enemy = GetComponent<Enemy>();
        }

        protected virtual void Update()
        {
            if(enemy.IsAlive)
                currentPattern?.Execute();
        }

        protected void FixedUpdate()
        {
            if(enemy.IsAlive)
                currentPattern?.FixedExecute();
        }

        public void ChangePattern(EnemyPattern pattern)
        {
            currentPattern?.Exit();
            currentPattern = pattern;
            currentPattern?.Enter();
            
            currentPatternString = currentPattern != null ? currentPattern.GetType().Name : "No Pattern";
        }
        
        public abstract void Init();


    }
}
