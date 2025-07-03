using System;
using UnityEngine;

namespace ToB.Entities
{
    public class FlyIdleState:EnemyPattern
    {
        public FlyIdleState(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
        }
    }
}
