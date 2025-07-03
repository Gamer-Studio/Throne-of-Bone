using System;
using UnityEngine;

namespace ToB.Entities
{
    public class FlyReturnState:EnemyPattern
    {
        public FlyReturnState(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
        }
    }
}
