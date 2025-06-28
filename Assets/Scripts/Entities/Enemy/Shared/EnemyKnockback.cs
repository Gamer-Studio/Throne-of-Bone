using System;
using System.Collections;
using UnityEngine;

namespace ToB.Entities
{
    public class EnemyKnockback : MonoBehaviour
    {
        private Enemy enemy;

        private readonly string KnockbackHash  = "Knockback";

        private void Awake()
        {
            if(!enemy) enemy = GetComponent<Enemy>();
        }

        private void Reset()
        {
            enemy = GetComponent<Enemy>();
        }

        public void ApplyKnockback(Vector2 direction, float force)
        {
            Vector2 knockbackVector = direction * force * enemy.EnemyData.BaseKnockbackMultiplier;
            enemy.Physics.externalVelocity.Add(KnockbackHash, knockbackVector);
            StartCoroutine(Knockback());
        }

        IEnumerator Knockback()
        {
            float duration = 0.5f;
            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                enemy.Physics.externalVelocity[KnockbackHash] = Vector2.Lerp(Vector2.zero, enemy.Physics.externalVelocity[KnockbackHash], elapsedTime / duration);
                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            enemy.Physics.externalVelocity.Remove(KnockbackHash);
        }
    }
}
