using System;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class SpikeTPPointSetter : MonoBehaviour
    {
        private Transform TPPosition;
        private Spikes Spike;
        private void Awake()
        {
            TPPosition = transform.GetChild(0);
            Spike = transform.parent.gameObject.GetComponent<Spikes>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Spike.SpikeTPTransform = TPPosition;
            }
        }
    }
}