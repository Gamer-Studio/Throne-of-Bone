using System;
using UnityEngine;

namespace ToB.Entities
{
    public class SewerRatPattern:EnemyPattern
    {
        protected readonly SewerRatStrategy ratStrategy;
        protected SewerRat sewerRat;

        protected SewerRatPattern(EnemyStrategy strategy, Action EndCallback) : base(strategy, EndCallback)
        {
            this.ratStrategy = strategy as SewerRatStrategy;
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
