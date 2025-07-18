using System;

namespace ToB.Entities
{
    public class EnemyStateTransition
    {
        public Func<bool> Condition { get; }
        public EnemyPattern Pattern { get; }
        
        public EnemyStateTransition(Func<bool> condition, EnemyPattern pattern)
        {
            Condition = condition;
            Pattern = pattern;
        }
    }
}