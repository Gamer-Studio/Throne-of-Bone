using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToB.Entities
{
    public abstract class EnemyPattern
    {
        protected readonly Enemy enemy;
        protected readonly EnemyStrategy strategy;
      
        List<EnemyStateTransition> transitions;
        event Action PatternEndCallback;    // 혹시 몰라서. 불필요하면 삭제
        
        /// <summary>
        /// FSM, 전략패턴 어느 형태에서든 쓰이는 걸 감안해서 패턴 종료시 콜백식으로 다음 상태 셋팅을 합니다.
        /// </summary>
        protected EnemyPattern(EnemyStrategy strategy, Action EndCallback = null)
        {
            this.strategy = strategy;
            this.enemy = strategy.enemy;
            PatternEndCallback += EndCallback;
            transitions = new List<EnemyStateTransition>();
        }

        public virtual void Enter()
        {
        }

        public virtual void Execute()
        {
            foreach (var transition in transitions)
            {
                if (transition.Condition())
                {
                    strategy.ChangePattern(transition.Pattern);
                    transition.OnTransition?.Invoke();
                }
            }
        }
        
        public virtual void FixedExecute()
        {
        }
        
        public virtual void LateExecute()
        {
        }

        public virtual void Exit()
        {
            PatternEndCallback?.Invoke();
        }

        public void AddTransition(Func<bool> condition, EnemyPattern pattern, Action onTransition = null)
        {
            transitions.Add(new EnemyStateTransition(condition, pattern, onTransition));
        }
    }
}
