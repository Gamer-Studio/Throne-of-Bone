using System;

namespace ToB.Entities
{
    public class EnemyStateTransition
    {
        public Func<bool> Condition { get; }
        public EnemyPattern Pattern { get; }
        public Action OnTransition { get; }
        
        public EnemyStateTransition(Func<bool> condition, EnemyPattern pattern, Action onTransition = null)
        {
            Condition = condition;
            Pattern = pattern;
            OnTransition = onTransition;
        }
    }
}