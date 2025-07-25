using System;
using DG.Tweening;
using ToB.Entities.Projectiles;
using ToB.Utils;
using UnityEngine;
using UnityEngine.VFX;

namespace ToB.Entities
{
    public class ReflectProjectile : MonoBehaviour
    {
        public LayerMask targetLayerAfterReflect;
        public LayerMask hitPointRaycastLayer;
        public GameObject effectPrefab;
        public float effectDuration = 0.85f;
        private Tween effectCleaner;

        private void Reset()
        {
            targetLayerAfterReflect = LayerMask.GetMask("Player");
            hitPointRaycastLayer = LayerMask.GetMask("Projectile");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 임시 특수 처리입니다. 발사체 규격화가 되면 좋습니다
            if (other.TryGetComponent<SwordEffect>(out var swordEffect))
            {
                swordEffect.direction = (swordEffect.transform.position - transform.position).normalized;
                swordEffect.hitLayers = targetLayerAfterReflect;
                
                RaycastHit2D hit = Physics2D.Raycast(transform.position, swordEffect.direction, 100, hitPointRaycastLayer);

                PlayEffect(hit.point);
                
            }
        }

        private void PlayEffect(Vector3 position)
        {
            GameObject effectObj = effectPrefab.Pooling();
            effectObj.transform.position = position;
            var effect = effectObj.GetComponent<VisualEffect>();
            effect.Play();
            effectCleaner = DOVirtual.DelayedCall(effectDuration, () =>
            {
                effect.Stop();
                effectObj.Release();
            });
        }
        
        private void OnDestroy()
        {
            effectCleaner?.Kill();
        }
    }
}
