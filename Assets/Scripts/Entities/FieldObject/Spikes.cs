using System.Transactions;
using ToB.Entities.Interface;
using ToB.Scenes.Stage;
using ToB.Worlds;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class Spikes : FieldObjectProgress, IAttacker
    {
        [SerializeField] private int damage;
        [field: SerializeField] public bool Blockable { get; set; }
        [field: SerializeField] public bool Effectable { get; set; }
        public Vector3 Position => transform.position;
        public Team Team => Team.Enemy;

        private void Awake()
        {
            Blockable = false;
            Effectable = true;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.Damage(damage,this);
                if (StageManager.Instance.player.stat.Hp > 0)
                    StageManager.Instance.player.TeleportByObject();
            }
        }

    }
}