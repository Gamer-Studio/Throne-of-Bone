using System;
using DG.Tweening;
using UnityEngine;

namespace ToB
{
    public class ScalePingPong : MonoBehaviour
    {
        public float scaleMultiplier = 1.2f;
        public float frequency = 0.5f;
        
        private Tween scaleTween;
        void OnEnable()
        {
            scaleTween = transform.DOScale(Vector3.one * scaleMultiplier, frequency)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        private void OnDisable()
        {
            scaleTween.Kill();
        }
    }
}
