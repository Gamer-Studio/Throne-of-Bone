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
        
        #region SaveLoad

        private void Awake()
        {
            if (rb ==null) rb = GetComponent<Rigidbody2D>();
            initialPos = transform.position;
            initialLocalPosition = spriteTransform.localPosition;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            if (hitbox ==null) hitbox = GetComponent<Collider2D>();
        }
        public override void LoadJson(JObject json)
        {
            base.LoadJson(json);
            isActivated = json.Get(nameof(isActivated), false);
        }
        
        public override void OnLoad()
        {
            gameObject.SetActive(!isActivated);
        }

        private void OnTriggerStay2D(Collider2D other)
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
                if ((shakeTween == null || !shakeTween.IsActive()) && !isActivated)
                {
                    shakeTween = spriteTransform.DOLocalMoveY(initialLocalPosition.y + 0.1f, 0.1f)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetEase(Ease.InOutSine);
                }
            }
            
        }

        private void OnTriggerExit2D(Collider2D other)
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
            yield return new WaitForSeconds(0.1f);
            while (rb.linearVelocity.magnitude > 0)
            {
                yield return new WaitForSeconds(0.2f);
            }
            transform.position = initialPos;
            gameObject.SetActive(false);
            yield return null;
        }
    }
}
