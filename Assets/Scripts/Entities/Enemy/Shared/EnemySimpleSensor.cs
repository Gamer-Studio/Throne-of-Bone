using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToB
{
    public class EnemySimpleSensor : MonoBehaviour
    {
        [SerializeField] private LayerMask targetMask;
        [SerializeField] List<GameObject> targets;
        public bool TargetInArea => targets.Count > 0;

        private void Awake()
        {
            targets = new List<GameObject>();
        }

        private void Reset()
        {
            targetMask = LayerMask.GetMask("Player");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((targetMask & (1 << other.gameObject.layer)) != 0)
            {
                targets.Add(other.gameObject);
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