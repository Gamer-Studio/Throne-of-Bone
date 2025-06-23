using System;
using UnityEngine;

namespace ToB
{
    public class SewerRatPattern:EnemyPattern
    {
        protected SewerRatStrategy strategy;
        
        public SewerRatPattern(Enemy enemy, SewerRatStrategy strategy, Action EndCallback) : base(enemy, EndCallback)
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
