using NaughtyAttributes;
using ToB.Entities.Interface;
using ToB.Utils;
using ToB.Worlds;
using UnityEngine;

namespace ToB.Entities.Projectiles
{
    public class Arrow : Projectile
    { 
      [ReadOnly] private new Camera camera;
      [Label("피해량")] public float damage;
      [Label("속도")] public float speed;
      [Label("넉백 세기")] public float knockBackForce;
      
      public LayerMask hitLayers;
      private float timeAfterLaunch = 0.3f;
      
      [SerializeField] private TrailRenderer trail;
      [SerializeField] private ParticleSystem ps;
      [SerializeField] private GameObject hitEffectPrefab;
      [SerializeField] private Rigidbody2D body;

      public Vector2 Direction
        {
          get => direction;
          set
        {
          direction = value;
          transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }
      }

      public void ClearEffect()
        {
          if(trail) trail.Clear();
          if(ps) ps.Clear();
        }
    
      #region Unity Event
    
        #if UNITY_EDITOR

        private void Reset()
        {
          if (!body) body = GetComponent<Rigidbody2D>();
        }
    
         #endif

     private void OnTriggerEnter2D(Collider2D other)
        {
          if ((hitLayers & 1 << other.gameObject.layer) == 0) return;
          
          // 발사된 뒤 시간이 얼마 지나지 않았을 경우 벽과의 충돌 무시 (발사되자마자 벽에 충돌해 사라짐 방지)
          if (other.gameObject.CompareTag("Ground") && timeAfterLaunch > 0) return;
      
         other.KnockBack(knockBackForce, direction);
      
          if (other.TryGetComponent<IDamageable>(out var damageable))
          {
            if (gameObject.activeSelf)
            {
              damageable.Damage(damage, this);
              //HitEffect(other);
            }
          }
          Release();
        }

        private void HitEffect(Collider2D other)
        {
          Vector2 otherCenter = other.GetComponent<Collider2D>().bounds.center;
          Vector2 posDiff = otherCenter - (Vector2)transform.position;
          Vector2 posDiffDir = posDiff.normalized;
          
          RaycastHit2D hit = Physics2D.Raycast(transform.position, posDiffDir, posDiff.magnitude, hitLayers);
          
          GameObject attackEffect = hitEffectPrefab.Pooling();
          attackEffect.transform.position = hit.point;
        
          float angle = Mathf.Atan2(posDiffDir.y, posDiffDir.x) * Mathf.Rad2Deg;
        
         var ps = attackEffect.GetComponent<ParticleSystem>().main;
         ps.startRotation = angle;
        
         attackEffect.gameObject.SetActive(true);
       }

       private void OnEnable()
        {
          camera = Camera.main;
          timeAfterLaunch = 0.3f;
          Team = Team.Enemy;
        }

       private void FixedUpdate()
        {
         body.MovePosition(body.position + direction * (speed * Time.fixedDeltaTime));
         
         // 발사 시 바로 벽에 부딪혀서 사라짐 방지
         if (timeAfterLaunch > 0)
         {
           timeAfterLaunch -= Time.deltaTime;
         }
      
         var pos = camera.WorldToViewportPoint(transform.position);

          if (pos.x < 0 || pos.x > 1 || pos.y < 0 || pos.y > 1)
          {
           Release();
          }
        }
        #endregion
    }
}