using System;

namespace ToB.Entities
{
    public class SmolSlimeMovePattern:GroundDefaultMovePattern
    {
        private readonly SmolSlime slime;

        public SmolSlimeMovePattern(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
            slime = enemy as SmolSlime;
        }

        public override void Execute()
        {
            base.Execute();
            if (slime.target)
                slime.FSM.ChangePattern(slime.FSM.chasePattern);
        }
    }
}