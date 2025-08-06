using DG.Tweening;
using UnityEngine;

namespace ToB
{
    public class ScalePingPong : MonoBehaviour
    {
        public float scaleMultiplier = 1.2f;
        public float frequency = 0.5f;
        void Start()
        {
            transform.DOScale(Vector3.one * scaleMultiplier, frequency)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }
}
