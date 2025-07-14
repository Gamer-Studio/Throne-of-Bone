using System;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class CoverTIleGradiation : MonoBehaviour
    {
        
        public enum GradiantDirection
        {
            Left,
            Right,
            Up,
            Down
        }
        
        public GradiantDirection direction;

        public SpriteRenderer Sr;

        private void Awake()
        {
            Sr = GetComponent<SpriteRenderer>();
        }
    }
}
