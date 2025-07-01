using System;
using UnityEngine;

namespace ToB.Entities
{
    public class SewerRatPattern:EnemyPattern
    {
        protected readonly SewerRatStrategy strategy;
        protected SewerRat sewerRat;

        protected SewerRatPattern(Enemy enemy, SewerRatStrategy strategy, Action EndCallback) : base(enemy, EndCallback)
        {
            this.strategy = strategy;
            sewerRat = enemy as SewerRat;
        }

        public override void Enter()
        {

        }

        public override void Execute()
        {

        }
    }
}
