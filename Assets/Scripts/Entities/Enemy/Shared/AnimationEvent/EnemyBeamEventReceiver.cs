using System.Collections.Generic;
using ToB.Entities;
using UnityEngine;

namespace ToB
{
    public class EnemyBeamEventReceiver : MonoBehaviour
    {
        public List<EnemyBeamAttack> beams;

        public void OnBeamAttack()
        {
            if (beams.Count == 0)
            {
                Debug.LogWarning("Beam is null");
            }

            foreach (var beam in beams)
            {
                beam.Attack();
            }
        }

        public void OnBeamLastAttack()
        {
            foreach (var beam in beams)
            {
                beam.LastAttack();
            }
        }
    }
}
