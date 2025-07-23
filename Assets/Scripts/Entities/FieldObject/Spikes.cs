using ToB.Entities.Interface;
using ToB.Scenes.Stage;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class Spikes : FieldObjectProgress
    {
        [SerializeField] private int damage;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                IDamageableExtensions.Damage(other.gameObject, (float)damage,(IAttacker)this);
                StageManager.Instance.player.TeleportByObstacle();
            }
        }
    }
}