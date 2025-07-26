using ToB.Entities.Interface;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class ContactDamage : MonoBehaviour
    {
        private IConatactDamageSO attackSO;
        public float ATK => attackSO?.ATK ?? nonSODamage;

        public float nonSODamage;
        public float knockBackPower;
        public Vector2 knockBackDirection;
        
        public LayerMask playerMask;

        private bool directional;

        private void Reset()
        {
            playerMask = LayerMask.GetMask("Player");
        }

        private void Awake()
        {
            directional = true;
        }

        public void Init(IConatactDamageSO attackSO = null, Vector2 knockBackDirection = default, bool fixedDirection = true)
        {
            this.attackSO = attackSO;
            this.knockBackDirection = knockBackDirection;
            directional = fixedDirection;
        }
        
        

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((playerMask & 1 << other.gameObject.layer) != 0)
            {
                IDamageableExtensions.Damage(other, ATK, (IAttacker)this);
                if(directional) other.KnockBack(knockBackPower, knockBackDirection);
                else other.KnockBack(knockBackPower, gameObject);
            }
        }
    }
}
