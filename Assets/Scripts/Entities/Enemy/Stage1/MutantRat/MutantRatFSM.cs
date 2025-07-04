using ToB.Entities;
using UnityEngine;

namespace ToB
{
    public class MutantRatFSM : EnemyStrategy
    {
        public MutantRatIdleState idleState;
        public MutantRatSleepState sleepState;
        public MutantRatRollState rollState;

        public override void Init()
        {
            idleState = new MutantRatIdleState(this);
            sleepState = new MutantRatSleepState(this);
            rollState = new MutantRatRollState(this);
            
            ChangePattern(sleepState);
        }
    }
}
