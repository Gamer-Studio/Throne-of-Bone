using ToB.Core;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class Chest_Small : MonoBehaviour, IDamageable
    {
        [SerializeField] private int gold;
        [SerializeField] private int mana;
        [SerializeField] private float HP;
        
        private Collider2D _collider;
        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;
        
        private void Awake()
        {
            gameObject.SetActive(true);
            _collider = GetComponent<Collider2D>();
            _rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        
        private void OnEnable()
        {
            HP = 1;
            _collider.enabled = true;
            _spriteRenderer.enabled = true;
        }
        public void Damage(float damage, MonoBehaviour sender = null)
        {
            HP -= damage;
            if (HP <= 0)
            {
                OnChestOpened();
            }
        }

        private void OnChestOpened()
        {
            Core.ResourceManager.Instance.SpawnResources(InfiniteResourceType.Gold, gold, transform);
            Core.ResourceManager.Instance.SpawnResources(InfiniteResourceType.Mana, mana, transform);
            _collider.enabled = false;
            _spriteRenderer.enabled = false;
            gameObject.SetActive(false);;
        }
        
    }
}