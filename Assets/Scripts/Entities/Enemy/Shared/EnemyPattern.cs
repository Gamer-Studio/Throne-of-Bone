using System;
using UnityEngine;

namespace ToB.Entities
{
    public abstract class EnemyPattern
    {
        protected readonly Enemy enemy;
      
        event Action PatternEndCallback;    // 혹시 몰라서. 불필요하면 삭제
        
        /// <summary>
        /// FSM, 전략패턴 어느 형태에서든 쓰이는 걸 감안해서 패턴 종료시 콜백식으로 다음 상태 셋팅을 합니다.
        /// </summary>
        protected EnemyPattern(Enemy enemy, Action EndCallback = null)
        {
            this.enemy = enemy;
            PatternEndCallback += EndCallback;
        }

        public virtual void Enter()
        {
        }

        public virtual void Execute()
        {
        }
        
        public virtual void FixedExecute()
        {
        }

        public virtual void Exit()
        {
            PatternEndCallback?.Invoke();
        }
    }
}
