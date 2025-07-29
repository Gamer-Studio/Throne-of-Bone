using System;
using NaughtyAttributes;
using ToB.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ToB
{
    public class GuardianShieldSensor : MonoBehaviour
    {
        [SerializeField, ReadOnly] private bool entered;
        public bool Entered => entered;
        private float lastEnterTime;

        public float senseDuration;
        public LayerMask targetLayer;

        private void Update()
        {
            if (Time.time - lastEnterTime > senseDuration)
            {
                entered = false;
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if((targetLayer | (1 << other.gameObject.layer)) == 0) return;

            if (other.TryGetComponent<PlayerCharacter>(out _))
            {
                entered = Random.Range(0, 100) < 50;
                if(entered) lastEnterTime = Time.time;
            }
            else
            {
                entered = true;
                lastEnterTime = Time.time;
            }
           
        }
    }
}
