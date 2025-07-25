using System.Collections;
using ToB.Utils;
using ToB.Entities.Projectiles;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ToB.Entities.FieldObject
{
    public class RockFallTrap : FieldObjectProgress
    {
        [SerializeField] private AssetReference RockPrefab;
        [SerializeField] private GameObject TrapSprite;
        [SerializeField] private Transform SpawnPos;
        public bool IsDetected;
        [SerializeField] private float FallDelayTime;
        [SerializeField] private float RockSpeedMultiplier;
        [SerializeField] private float RockDamage;
        [SerializeField] private float RockKnockBack;
        public bool IsActivated;
        private Coroutine shootingCo;
        public override void OnLoad()
        {
            IsActivated = false;
            IsDetected = false;
        }
        
        private void Update()
        {
            // IsDetected는 감지 콜라이더가 관리
            if (IsDetected && !IsActivated)
            {
                IsActivated = true;
                StartCoroutine(Shoot());
            }
        }

        private IEnumerator Shoot()
        {
            //딜레이 시간 동안 대기한 뒤 발사
            yield return new WaitForSeconds(FallDelayTime);
            var eff = RockPrefab.Pooling().GetComponent<Rock>();
            eff.transform.position = SpawnPos.position;
            eff.Direction = Vector2.down;
            eff.damage = RockDamage;
            eff.knockBackForce = RockKnockBack;
            eff.speed = RockSpeedMultiplier;
            
            eff.ClearEffect();
        }

    }
}