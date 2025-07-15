using UnityEngine;
using DG.Tweening;
using ToB.Core;
using ToB.Player;

namespace ToB.Entities.Effect
{
    public class ResourceDropping : MonoBehaviour
    {
        
        private bool isCollected = false;
        private bool isCollectableByPlayer = false;
        
        public int amount;
        public InfiniteResourceType resourceType;
        
        [SerializeField] public float jumpForce = 12.5f;
        [SerializeField] public float autoCollectDelay = 2f;
        [SerializeField] private float collectSpeed = 10f;
        [SerializeField] private float collectAcceleration = 50f;
        [SerializeField] private GameObject obtainEffectPrefab;
        
        //[SerializeField] public float collectMoveDuration = 1f;

        private Rigidbody2D rb;
        private Collider2D _collider;
        private Vector2 currentVelocity;
        private PlayerCharacter player;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            player = PlayerCharacter.GetInstance();
            _collider = GetComponent<Collider2D>();
        }

        private void OnEnable()
        {
            float angle = Random.Range(-15f, 15f);
            float rad = angle * Mathf.Deg2Rad;
            float force = Random.Range(0.8f, 1.2f) * jumpForce;
            Vector2 direction = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)).normalized;
            rb.AddForce(direction * force, ForceMode2D.Impulse);
            Invoke(nameof(AutoCollect), autoCollectDelay);
        }
        private void OnDisable()
        {
            isCollected = false;
            isCollectableByPlayer = false;
            rb.simulated = true;
            _collider.isTrigger = false;
        }

        private void Update()
        {
            if (isCollectableByPlayer)
            {
                Vector2 direction = (player.transform.position - transform.position);
                if (direction.sqrMagnitude < 0.1f)
                {
                    if (resourceType == InfiniteResourceType.Gold)
                        Core.ResourceManager.Instance.GiveGoldToPlayer(amount);
                    else
                        Core.ResourceManager.Instance.GiveManaToPlayer(amount);
                    gameObject.Release();
                    GameObject effect = obtainEffectPrefab.Pooling();
                    effect.transform.position = player.transform.position;
                }
                
                currentVelocity = Vector2.Lerp(currentVelocity, direction * collectSpeed, collectAcceleration * Time.deltaTime);
                transform.position += (Vector3)(currentVelocity * Time.deltaTime);
            }
        }

        private void AutoCollect()
        {
            if (isCollected) return;
            isCollected = true;

            rb.simulated = false; // 물리 비활성화
            _collider.isTrigger = true;
            isCollectableByPlayer = true; 

            Vector2 direction = (player.transform.position - transform.position).normalized;
            currentVelocity = direction * collectSpeed * 0.2f;
        }
        
    }
}