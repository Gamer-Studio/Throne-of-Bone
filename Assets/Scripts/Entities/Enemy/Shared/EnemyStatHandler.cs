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
        private IEnemyHittableSO hittableSO;
        
        [SerializeField, ReadOnly] private float currentHP;
        private float CurrentHP => currentHP;
        private float MaxHP => hittableSO.HP;
        public float DEF => hittableSO.DEF;

        public Coroutine DamageEffectCoroutine { get; private set; }
        public bool OnDamageEffect { get; private set; }
        
        /// <summary>
        /// 아직까지는 본체가 HP를 갖기에 본체의 데이터가 고스란히 들어가지만
        /// 분할부위나 여러 마공핵 파괴 방식 같은 기믹이 나올 경우 루트 SO의 인터페이스 하나로는 부족하기에
        /// 루트 SO 안에 Hittable 인터페이스를 상속받은 복수의 SO를 리스트로 다루는 경우까지 감안하여
        /// 2번째 파라미터를 따로 마련했습니다.
        /// </summary>
        /// <returns></returns>
        public void Init(Enemy enemy, IEnemyHittableSO hittableSO)
        {
            this.enemy = enemy;

            if (hittableSO == null)
            {
                Debug.Log("SO가 hittable 인터페이스를 상속받지 못했습니다. " + enemy.gameObject.name);
                return;
            }
            this.hittableSO = hittableSO;
            currentHP = MaxHP;
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
            float actualDamage = damage * (100 - DEF) / 100;
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

        public void SetDefault()
        {
            currentHP = MaxHP;
        }
    }
}
