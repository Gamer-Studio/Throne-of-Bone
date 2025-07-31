using System;
using System.Collections.Generic;
using ToB.Entities.Interface;
using ToB.Worlds;
using UnityEngine;

namespace ToB.Entities
{
    public class EnemyBeamAttack:MonoBehaviour, IAttacker
    {
        public List<GameObject> objects;

        private float damage;
        private float endDamage;

        public bool Blockable { get; private set;  }
        public bool Effectable { get; private set; }
        public Vector3 Position { get; private set; }
        public Team Team { get; private set; }

        private void Awake()
        {
            objects = new List<GameObject>();
            Team = Team.Enemy;
            Blockable = false;
        }

        private void OnDisable()
        {
            objects.Clear();       
        }

        public void Init(float damage, float endDamage = 0)
        {
            this.damage = damage;
            this.endDamage = endDamage;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            objects.Add(other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            objects.Remove(other.gameObject);
            // TODO : 플레이어 무적 설정
        }

        public void Attack()
        {
            foreach (var obj in objects)
            {
                obj.Damage(damage, this);
                obj.KnockBack(1, new  Vector2(obj.transform.eulerAngles.y == 0 ? 1 : -1, 0));
            }
        }

        public void LastAttack()
        {
            foreach (var obj in objects)
            {
                obj.Damage(endDamage);
                obj.KnockBack(8, new  Vector2(obj.transform.eulerAngles.y == 0 ? 1 : -1, 0));
            }
        }
    }
}