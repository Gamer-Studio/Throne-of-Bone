using UnityEngine;

namespace ToB.Entities
{
    public class SecurityArcherAnimationEvent:MonoBehaviour
    {
        public SecurityArcherFSM fsm;

        private void SetPositionToSiege()
        {
            fsm.siegeTrigger = true;
        }
    }
}