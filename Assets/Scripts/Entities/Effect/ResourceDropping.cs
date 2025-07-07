using UnityEngine;
using DG.Tweening;
using ToB.Player;
using UnityEngine.ResourceManagement;
using UnityEngine.SceneManagement;

namespace ToB.Entities.Effect
{
    public class ResourceDropping : MonoBehaviour
    {
        private Rigidbody2D rb;
        private bool isCollected = false;
        [SerializeField] public float jumpForce = 3f;
        [SerializeField] public float autoCollectDelay = 1.5f;
        [SerializeField] public float collectMoveDuration = 0.5f;
        
        private PlayerCharacter player;
        
        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            rb = GetComponent<Rigidbody2D>();
        }
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "Intro" && scene.name != "MainMenu")
            {
                gameObject.SetActive(true);
                player = PlayerCharacter.GetInstance();
            }
        }

        private void Start()
        {
            // 살짝 위로 튀어오르며 생성
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            // 일정 시간 후 자동 수집
            Invoke(nameof(AutoCollect), autoCollectDelay);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!isCollected && collision.gameObject.CompareTag("Ground"))
            {
                // 바닥에 닿으면 멈춤
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.simulated = false;
            }
        }

        private void AutoCollect()
        {
            if (isCollected) return;
            isCollected = true;
            rb.simulated = false; // 물리 비활성화

            // DOTween으로 플레이어에게 흡수
            transform.DOMove(player.transform.position, collectMoveDuration)
                .SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    Core.ResourceManager.Instance.GiveGoldToPlayer(10);
                    Destroy(gameObject);
                });
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isCollected && other.CompareTag("Player"))
            {
                AutoCollect();
            }
        }

        
    }
}