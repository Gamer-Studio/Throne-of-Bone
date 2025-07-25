using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class Trampoline : FieldObjectProgress
    {
        [SerializeField] private float Player_JumpForce;
        [SerializeField] private float Box_JumpForce;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                other.TryGetComponent<Rigidbody2D>(out var _rb);
                if (_rb == null) return;
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x/2f, Player_JumpForce);
            }
            else if (other.CompareTag("Box"))
            {
                other.TryGetComponent<Rigidbody2D>(out var _rb);
                if (_rb == null) return;
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x/2f, Box_JumpForce);
            }
        }
    }
}