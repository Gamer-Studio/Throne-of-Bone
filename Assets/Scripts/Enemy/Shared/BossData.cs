using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToB
{
    [CreateAssetMenu(fileName = "BossData", menuName = "Scriptable Objects/BossData")]
    public class BossData : EnemyData
    {
        [field: SerializeField] public List<string> dialogs { get; private set; }
    }
}
