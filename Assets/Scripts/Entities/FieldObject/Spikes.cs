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
                other.gameObject.Damage(damage,this);
                StageManager.Instance.player.TeleportByObject();
            }
        }
    }
}