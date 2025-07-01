using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace ToB.Entities
{
    
    public class EnemyKnockback : MonoBehaviour, IKnockBackable
    {
        [SerializeField, ReadOnly]private Enemy enemy;

        private readonly string KnockbackHash  = "Knockback";
        [SerializeField, ReadOnly] private float knockbackMultiplier;
        
        Coroutine knockbackCoroutine;
        
        public void Init(Enemy enemy, float knockbackMultiplier)
        {
            this.enemy = enemy;
            this.knockbackMultiplier = knockbackMultiplier;
        }

        public void KnockBack(float force, Vector2 direction)
        {
            if (!enemy)
            {
                Debug.Log("넉백 컴포넌트를 초기화해주세요 : " + gameObject.name);
                return;
            }
            Vector2 knockbackVector = direction * force * knockbackMultiplier;
           
            
            if(knockbackCoroutine != null) StopCoroutine(knockbackCoroutine);
            knockbackCoroutine = StartCoroutine(Knockback(knockbackVector));
        }

        IEnumerator Knockback(Vector2 knockbackVector)
        {
            enemy.Physics.externalVelocity[KnockbackHash] = knockbackVector;
            float duration = 0.5f;
            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                enemy.Physics.externalVelocity[KnockbackHash] = Vector2.Lerp(enemy.Physics.externalVelocity[KnockbackHash], Vector2.zero, elapsedTime / duration);
                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            enemy.Physics.externalVelocity.Remove(KnockbackHash);
        }

        public void KnockBack(float value, GameObject sender) => KnockBack(value, transform.position - sender.transform.position);

    }
}
