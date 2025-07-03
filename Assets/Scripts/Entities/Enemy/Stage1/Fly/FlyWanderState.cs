using System;
using UnityEngine;

namespace ToB.Entities
{
    public class FlyWanderState:EnemyPattern
    {
        public FlyWanderState(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
        }
    }
}
