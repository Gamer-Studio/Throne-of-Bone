using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace ToB.Entities
{
    public class EnemyKnockback : MonoBehaviour, IKnockBackable
    {
        [SerializeField, ReadOnly] private Enemy enemy;
        private IEnemyKnockbackSO knockbackSO;

        private readonly string KnockbackHash = "Knockback";
        float KnockbackMultiplier => knockbackSO.KnockbackMultiplier;

        Coroutine knockbackCoroutine;

        public bool isActive;

        public void Init(Enemy enemy)
        {
            this.enemy = enemy;
            knockbackSO = enemy.EnemySO as IEnemyKnockbackSO;
            isActive = true;
        }

        public void KnockBack(float force, Vector2 direction)
        {
            if (!enemy)
            {
                Debug.Log("넉백 컴포넌트를 초기화해주세요 : " + gameObject.name);
                return;
            }

            if (!isActive) return;
            
            Vector2 knockbackVector = direction * force * KnockbackMultiplier;

            if (knockbackCoroutine != null) StopCoroutine(knockbackCoroutine);
            knockbackCoroutine = StartCoroutine(Knockback(knockbackVector));
        }

        IEnumerator Knockback(Vector2 knockbackVector)
        {
            enemy.Physics.externalVelocity[KnockbackHash] = knockbackVector;
            float duration = 0.5f;
            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                enemy.Physics.externalVelocity[KnockbackHash] =
                    Vector2.Lerp(enemy.Physics.externalVelocity[KnockbackHash], Vector2.zero, elapsedTime / duration);
                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            enemy.Physics.externalVelocity.Remove(KnockbackHash);
        }

        public void KnockBack(float value, GameObject sender) =>
            KnockBack(value, transform.position - sender.transform.position);
    }
}