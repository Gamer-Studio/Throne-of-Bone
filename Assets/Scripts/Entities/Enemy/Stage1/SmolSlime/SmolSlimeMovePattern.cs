using System;

namespace ToB.Entities
{
    public class SmolSlimeMovePattern:GroundDefaultMovePattern
    {
        private readonly SmolSlime slime;
        public SmolSlimeMovePattern(Enemy enemy, float moveSpeed, Action EndCallback = null) : base(enemy, moveSpeed, EndCallback)
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