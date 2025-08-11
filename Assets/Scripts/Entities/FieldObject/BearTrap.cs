using System.Collections;
using ToB.Entities.Interface;
using ToB.Utils;
using ToB.Worlds;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class BearTrap : FieldObjectProgress, IAttacker
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
        private ObjectAudioPlayer audioPlayer; 
        
        private void Awake()
        {
            playerMask = LayerMask.GetMask("Player");       
            if(!animator) animator = GetComponentInChildren<Animator>();
            state = State.Opened;
            audioPlayer = GetComponent<ObjectAudioPlayer>();
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
                
                other.Damage(damage, this);
                other.KnockBack(knockBackPower, new Vector2(other.transform.eulerAngles.y == 0 ? 1 : -1, 1));
                animator.SetTrigger(ObstacleAnimationString.Activate);
                audioPlayer.Play("env_trap_activate_01");
                state = State.Closed;
                StartCoroutine(ResetTrap());
            }
        }

        IEnumerator ResetTrap()
        {
            yield return new WaitForSeconds(resetDuration);
            state = State.Opened;
            //animator.SetTrigger(ObstacleAnimationString.Reset);
            
        }

        [field:SerializeField] public bool Blockable { get; set; }
        [field:SerializeField] public bool Effectable { get; set; }
        public Vector3 Position => transform.position;
        public Team Team => Team.None;
    }
}
