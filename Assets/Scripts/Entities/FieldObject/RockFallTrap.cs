using System.Collections;
using DG.Tweening;
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
        private Tween vibrateTween;
        public override void OnLoad()
        {
            TrapSprite.SetActive(true);
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
            FallingWarning();
            yield return new WaitForSeconds(FallDelayTime);
            var eff = RockPrefab.Pooling().GetComponent<Rock>();
            eff.transform.position = SpawnPos.position;
            eff.Direction = Vector2.down;
            eff.damage = RockDamage;
            eff.knockBackForce = RockKnockBack;
            eff.speed = RockSpeedMultiplier;
            
            eff.ClearEffect();
            
            TrapSprite.SetActive(false);
        }

        private void FallingWarning()
        {
            TrapSprite.transform.DOShakePosition(
                    duration: FallDelayTime
                    , strength: new Vector3(0.1f, 0.1f, 0)
                    , vibrato: 60
                    , randomness: 90
                    , fadeOut: true
                )
                .OnComplete(() => vibrateTween.Kill());
        }

    }
}