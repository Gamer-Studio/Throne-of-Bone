
using System;
using UnityEngine;

namespace ToB.Entities
{
    public class FlyChaseState:EnemyPattern
    {
        public FlyChaseState(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
        }
    }
}
