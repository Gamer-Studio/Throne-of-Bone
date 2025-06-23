using System;
using UnityEngine;

namespace ToB
{
    public class SewerRatScratchPattern : SewerRatPattern
    {
        /// <inheritdoc/>
        public SewerRatScratchPattern(Enemy enemy, SewerRatStrategy strategy, Action EndCallback) : base(enemy, strategy, EndCallback)
        {
        }

        public override void Enter()
        {

        }

        public override void Execute()
        {
            Exit();
        }

        protected override void Exit()
        {
            base.Exit();
        }
    }
}