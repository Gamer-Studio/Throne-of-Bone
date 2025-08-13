using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace ToB.Entities
{
    public class EnemySimpleSensor : MonoBehaviour
    {
        [SerializeField] private LayerMask targetMask;
        [SerializeField] List<GameObject> targets;
        public bool TargetInArea => targets.Count > 0;
        [ReadOnly] public bool entered;
        [ReadOnly] public float lastEnteredTime;

        public float senseDuration;

        private void Awake()
        {
            targets = new List<GameObject>();
        }

        private void Reset()
        {
            targetMask = LayerMask.GetMask("Player");
        }

        private void Update()
        {
            if (Time.time - lastEnteredTime > senseDuration)
            {
                entered = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((targetMask & (1 << other.gameObject.layer)) != 0)
            {
                targets.Add(other.gameObject);
                entered = true;
                lastEnteredTime = Time.time;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (targets.Contains(other.gameObject))
            {
                targets.Remove(other.gameObject);
            }
        }
    }
}