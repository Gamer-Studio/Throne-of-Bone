using System.Collections;
using ToB.Entities.Interface;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class BearTrap : FieldObjectProgress
    {
        private enum State
        {
            Opened,
            Closed
        }
        
        [SerializeField] private Animator animator;
        private LayerMask playerMask;
        [SerializeField] State state;
        [SerializeField] private float knockBackPower;
        [SerializeField] private float damage;

        [SerializeField] private float resetDuration = 3;
         
        private void Awake()
        {
            playerMask = LayerMask.GetMask("Player");       
            if(!animator) animator = GetComponentInChildren<Animator>();
            state = State.Opened;
        }

        private void Reset()
        {
            animator = GetComponentInChildren<Animator>();       
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((playerMask & 1 << other.gameObject.layer) != 0)
            {
                if (state != State.Opened) return;
                
                IDamageableExtensions.Damage(other, damage, (IAttacker)this);
                other.KnockBack(knockBackPower, new Vector2(other.transform.eulerAngles.y == 0 ? 1 : -1, 1));
                animator.SetTrigger(ObstacleAnimationString.Activate);
                state = State.Closed;
                StartCoroutine(ResetTrap());
            }
        }

        IEnumerator ResetTrap()
        {
            yield return new WaitForSeconds(resetDuration);
            state = State.Opened;
            animator.SetTrigger(ObstacleAnimationString.Reset);
            
        }
        
    }
}
