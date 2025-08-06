using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
using ToB.Entities.Interface;
using ToB.Utils;
using ToB.Worlds;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ToB.Entities.Projectiles
{
    public class Rock :Projectile
    { 
      //[ReadOnly] private new Camera camera;
      [Label("속도")] public float speed;
      [Label("넉백 세기")] public float knockBackForce;
      
      public LayerMask hitLayers;
      
      [SerializeField] private TrailRenderer trail;
      [SerializeField] private ParticleSystem ps;
      [SerializeField] private GameObject hitEffectPrefab;
      [SerializeField] public Rigidbody2D body;
      
      [Header("깜빡임 두트윈")]
      [SerializeField] private TilemapRenderer spriteRenderer;
      [SerializeField] private float blinkAlpha;
      [SerializeField] private float blinkDuration;
      [SerializeField] private int blinkCount;
      private Tween blinkTween;
      private Material tilemapMaterialInstance;
      private bool IsContacted;
      private Color originalColor;
      public override bool Blockable => false;
      public Vector2 Direction
        {
          get => direction;
          set
        {
          direction = value;
          //transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }
      }

      public void ClearEffect()
        {
          if(trail) trail.Clear();
          if(ps) ps.Clear();
        }
    
    
        #if UNITY_EDITOR

        private void Reset()
        {
          if (!body) body = GetComponent<Rigidbody2D>();
        }
    
         #endif
    
       protected override void OnTriggerEnter2D(Collider2D other)
        {
          if ((hitLayers & 1 << other.gameObject.layer) == 0) return;
          if (IsContacted) return;
          
          Debug.Log("충돌 시퀀스 실행");
      
          other.KnockBack(knockBackForce, direction);
      
          if (other.TryGetComponent<IDamageable>(out var damageable))
          {
            if (gameObject.activeSelf)
            {
              damageable.Damage(damage, this);
              //HitEffect(other);
            }
          }
          //플레이어 혹은 바닥에 충돌 시, 사라짐 코루틴 시작
          StartCoroutine(Disappear());
        }
       
       private IEnumerator Disappear()
        {
          Debug.Log("코루틴 시작");
          IsContacted = true;
          trail.enabled = false; // 트레일 제거
          // 0.5초 최소 시간 보장
          yield return new WaitForSeconds(0.5f);
          while (body.linearVelocity.y > 0.2f)
          {
            yield return new WaitForFixedUpdate();
          }
          //깜빡임 시작. 끝나면 풀로 반환
          StartBlink();
        }
        public void StartBlink()
        {
          blinkTween?.Kill();

          originalColor = tilemapMaterialInstance.color;
          Color blinkColor = originalColor;
          blinkColor.a = blinkAlpha;

          blinkTween = tilemapMaterialInstance
            .DOColor(blinkColor, blinkDuration)
            .SetLoops(blinkCount * 2, LoopType.Yoyo)
            .OnComplete(ResetRock);
        }
        private void ResetRock()
        {
          Release();
          tilemapMaterialInstance.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1);
          IsContacted = false;
        }
        
       // 추후 이펙트 추가 시 이용
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

        private void Start()
        {
          body.AddForce(Vector2.down * speed, ForceMode2D.Impulse);
        }
        private void OnEnable()
        {
          //camera = Camera.main;
          trail.enabled = true;
          tilemapMaterialInstance = Instantiate(spriteRenderer.material);
          spriteRenderer.material = tilemapMaterialInstance;
          body.AddForce(Vector2.down * speed, ForceMode2D.Impulse);
          Team = Team.Enemy;
        }

       /*
       private void FixedUpdate()
        {
         // body.MovePosition(body.position + direction * (speed * Time.fixedDeltaTime));
      
         var pos = camera.WorldToViewportPoint(transform.position);

          if (pos.x < 0 || pos.x > 1 || pos.y < 0 || pos.y > 1)
          {
           Release();
          }
        }
        */
    }
}