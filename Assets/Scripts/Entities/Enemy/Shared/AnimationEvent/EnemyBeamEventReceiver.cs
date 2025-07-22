using ToB.Entities;
using UnityEngine;

namespace ToB
{
    public class EnemyBeamEventReceiver : MonoBehaviour
    {
        public EnemyBeamAttack beam;

        public void OnBeamAttack()
        {
            if (!beam)
            {
                Debug.LogWarning("Beam is null");
            }
            beam.Attack();
        }

        public void OnBeamLastAttack()
        {
            beam.LastAttack();
        }
    }
}
