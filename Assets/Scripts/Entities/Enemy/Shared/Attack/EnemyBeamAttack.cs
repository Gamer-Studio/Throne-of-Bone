using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToB.Entities
{
    public class EnemyBeamAttack:MonoBehaviour
    {
        private List<GameObject> objects;

        private float damage;
        private float endDamage;

        private void Awake()
        {
            objects = new List<GameObject>();
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
                obj.Damage(damage);
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