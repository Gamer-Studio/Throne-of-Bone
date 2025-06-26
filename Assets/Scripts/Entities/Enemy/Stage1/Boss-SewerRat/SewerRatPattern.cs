using System;
using UnityEngine;

namespace ToB
{
    public class SewerRatPattern:EnemyPattern
    {
        protected readonly SewerRatStrategy strategy;

        protected SewerRatPattern(Enemy enemy, SewerRatStrategy strategy, Action EndCallback) : base(enemy, EndCallback)
        {
            this.strategy = strategy;
        }

        public override void Enter()
        {

        }

        public override void Execute()
        {

        }
    }
}
