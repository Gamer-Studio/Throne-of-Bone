using ToB.Entities.Interface;
using ToB.Scenes.Stage;
using ToB.Worlds;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class Spikes : FieldObjectProgress, IAttacker
    {
        [SerializeField] private int damage;
        public bool Blockable => false;
        public bool Effectable => true;
        public Vector3 Position => transform.position;
        public Team Team => Team.Enemy;
        
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