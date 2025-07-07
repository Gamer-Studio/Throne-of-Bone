using UnityEngine;
using DG.Tweening;

namespace ToB.Entities.Effect
{
    public class ResourceDropping : MonoBehaviour
    {
        [SerializeField] private float fallDistance = 1.5f;
        [SerializeField] private float fallTime = 0.4f;
        [SerializeField] private float absorbDelay = 0.3f;
        [SerializeField] private float absorbTime = 0.5f;

        private Transform playerTransform;
        private SpriteRenderer spriteRenderer;
        private Sequence seq;

        public void Initialize(Transform player)
        {
            playerTransform = player;
            spriteRenderer = GetComponent<SpriteRenderer>();

            PlayDropAnimation();
        }

        private void PlayDropAnimation()
        {
            Vector3 fallTarget = transform.position - new Vector3(0, fallDistance, 0);

            seq = DOTween.Sequence();
            seq.Append(transform.DOMove(fallTarget, fallTime).SetEase(Ease.InQuad));
            seq.AppendInterval(absorbDelay);
            seq.Append(transform.DOMove(playerTransform.position, absorbTime).SetEase(Ease.InQuad))
                .Join(transform.DORotate(new Vector3(0, 0, 720), absorbTime, RotateMode.FastBeyond360))
                .OnComplete(OnAbsorbComplete);
        }

        private void OnAbsorbComplete()
        {
            // 골드 획득 처리 (플레이어 골드 증가 처리 연결)
            Debug.Log($"골드 흡수 완료: {gameObject.name}");
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            seq?.Kill();
        }
    }
}