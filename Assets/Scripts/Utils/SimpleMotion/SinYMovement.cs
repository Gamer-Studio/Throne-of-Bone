using System;
using UnityEngine;

namespace ToB.Utils
{
    public class SinYMovement : MonoBehaviour
    {
        Vector2 basePos;
        public float height;
        public float frequency;

        private void Awake()
        {
            basePos = transform.localPosition;
        }
        
        private void Update()
        {
            transform.localPosition = basePos + new Vector2(0, Mathf.Sin(Time.time * frequency * Mathf.PI) * height);
        }
    }
}
