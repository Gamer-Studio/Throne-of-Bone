using ToB.Core;
using ToB.Entities.Interface;
using ToB.Utils;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class Chest_Small : FieldObjectProgress, IDamageable
    {
        [SerializeField] private int gold;
        [SerializeField] private int mana;
        [SerializeField] private float HP;
        [SerializeField] private Animator animator;
        private Collider2D _collider;
        private Rigidbody2D _rb;
        private ObjectAudioPlayer audioPlayer;
        
        private void OnEnable()
        {
            gameObject.SetActive(true);
            if (_collider == null) _collider = GetComponent<Collider2D>();
            if (_rb == null) _rb = GetComponent<Rigidbody2D>();
            _rb.constraints = RigidbodyConstraints2D.FreezeAll;
            if (animator == null) animator = GetComponent<Animator>();
            if (audioPlayer == null) audioPlayer = GetComponent<ObjectAudioPlayer>();
            animator.SetBool("IsOpened", false);
            HP = 1;
            _collider.enabled = true;
        }
        public void Damage(float damage, IAttacker sender = null)
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
            animator.SetBool("IsOpened", true);
            audioPlayer.Play("Wood_04");
        }
        
    }
}