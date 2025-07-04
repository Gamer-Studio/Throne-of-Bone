using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace ToB.Entities.Obstacle
{
    public class FragilePlatform : MonoBehaviour
    {
        [SerializeField] public float fallCountdown = 3f;
        [SerializeField] private float fallTimer;
        //isActivated : 진행도 저장 여부
        public bool isActivated;
        private Vector3 initialPos;
        [SerializeField] public Transform spriteTransform; 
        private Rigidbody2D rb;
        private Tween shakeTween;
        private Vector3 initialLocalPosition;
        private Collider2D hitbox;
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            initialPos = transform.position;
            initialLocalPosition = spriteTransform.localPosition;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            hitbox = GetComponent<Collider2D>();
            if(isActivated) gameObject.SetActive(false);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player") && !isActivated)
            {
                fallTimer += Time.deltaTime;
                if (fallTimer >= fallCountdown)
                {
                    fallTimer = 0f;
                    StopShaking();
                    ActivateFall();
                }
            }
            if ((shakeTween == null || !shakeTween.IsActive()) && !isActivated)
            {
                shakeTween = spriteTransform.DOLocalMoveY(initialLocalPosition.y + 0.1f, 0.1f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                fallTimer = 0f;
                StopShaking();
            }
        }

        private void StopShaking()
        {
            if (shakeTween != null && shakeTween.IsActive())
            {
                shakeTween.Kill();
                shakeTween = null;
            }

            spriteTransform.localPosition = initialLocalPosition;
        }

        private void ActivateFall()
        {
            StartCoroutine(Activated());
        }

        private IEnumerator Activated()
        {
            isActivated = true;
            rb.gravityScale = 1f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            hitbox.enabled = false;
            StopShaking();
            yield return new WaitForSeconds(2f);
            transform.position = initialPos;
            gameObject.SetActive(false);
        }
    }
}
