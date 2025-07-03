using System;
using ToB.Entities;
using UnityEngine;

namespace ToB.Entities
{
    public class FlyAttackState:EnemyPattern
    {
        public FlyAttackState(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
        }
    }
}
