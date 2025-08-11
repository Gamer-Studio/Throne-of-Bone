using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class PopUpBlade_Collider : MonoBehaviour
    {
        [SerializeField] private PopUpBlade popUpBlade;
        
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && popUpBlade.isActivated && popUpBlade.invincibleTime <= 0)
            {
                other.Damage(popUpBlade.damage, popUpBlade);
                other.KnockBack(popUpBlade.knockBackPower, gameObject);
                popUpBlade.invincibleTime = 1f;
            }
        }
        
    }
}