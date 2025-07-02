using System;

namespace ToB.Entities
{
    public class SmolSlimeMovePattern:GroundDefaultMovePattern
    {
        private readonly SmolSlime slime;
        private SmolSlimeFSM fsm;

        public SmolSlimeMovePattern(EnemyStrategy strategy, float moveSpeed, Action EndCallback = null) : base(strategy, moveSpeed, EndCallback)
        {
            fsm = strategy as SmolSlimeFSM;
            this.slime = fsm.Owner;
        }

        public override void Execute()
        {
            base.Execute();
            if (slime.target)
                slime.FSM.ChangePattern(slime.FSM.chasePattern);
        }
    }
}