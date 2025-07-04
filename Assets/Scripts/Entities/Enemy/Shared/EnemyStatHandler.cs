using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace ToB.Entities
{
    
    // 지금은 안 쓰고 적군이 방어력 버프를 쓰는 등 처리가 복잡해질 때를 감안해서 보류
    public class EnemyStatHandler : MonoBehaviour, IDamageable
    {
        private Enemy enemy;
        
        [SerializeField, ReadOnly] private float MaxHP;
        [SerializeField, ReadOnly] private float currentHP;
        [SerializeField, ReadOnly] private float def;
        public float CurrentHP => currentHP;

        public Coroutine DamageEffectCoroutine { get; private set; }
        public bool OnDamageEffect { get; private set; }
        public void Init(Enemy enemy, float hp, float def = 0)
        {
            this.enemy = enemy;
            MaxHP = hp;
            currentHP = MaxHP;
            this.def = def;
        }
        
        private void ChangeHP(float delta)
        {
            currentHP += delta;
            currentHP = Mathf.Clamp(CurrentHP, 0, MaxHP);

            if (currentHP <= 0)
            {
                enemy.PartDestroyed(this);
            }
        }

        public void Damage(float damage, MonoBehaviour sender = null)
        {
            float actualDamage = damage * (100 - def) / 100;
            ChangeHP(-actualDamage);
            enemy.OnTakeDamage(sender);
            
            DamageEffectCoroutine = StartCoroutine(DamageColorOverlay());
        }
        
        IEnumerator DamageColorOverlay()
        {
            OnDamageEffect = true;
            enemy.Sprite.material.SetFloat("_Alpha", 1);
            float duration = 0.3f;
            float remainedTime = duration;

            while (remainedTime > 0)
            {
                yield return null;
                remainedTime -= Time.deltaTime;
                if (remainedTime < 0) remainedTime = 0;
                enemy.Sprite.material.SetFloat("_Alpha", remainedTime / duration);
            }
            OnDamageEffect = false;
        }

        private void OnDestroy()
        {
            if(DamageEffectCoroutine != null) StopCoroutine(DamageEffectCoroutine);
        }
    }
}
