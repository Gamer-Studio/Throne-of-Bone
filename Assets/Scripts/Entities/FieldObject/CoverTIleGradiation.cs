using System;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class CoverTIleGradiation : MonoBehaviour
    {
        
        //TODO : 폴리싱 때 섀이더그래프를 어케 해와서 작업할 것
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
