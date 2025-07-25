using System;
using NaughtyAttributes;
using ToB.Utils;
using UnityEngine;

namespace ToB.Entities.Projectiles
{
  public class SwordEffect : Projectile
  {
    [ReadOnly] private new Camera camera;
    [Label("피해량")] public float damage;
    [Label("속도")] public float speed = 1;
    [Label("넉백 세기")] public float knockBackForce = 15;
    
    public LayerMask hitLayers;

    private LayerMask hitLayersDefault;
    
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private GameObject hitEffectPrefab;

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

    public override void Release()
    {
      launcher = null;
      base.Release();
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
      if (!gameObject.activeSelf) return;
      if ((hitLayers & 1 << other.gameObject.layer) == 0) return;
      
      other.KnockBack(knockBackForce, direction);
      
      if (other.TryGetComponent<IDamageable>(out var damageable))
      {
        if (gameObject.activeSelf)
        {
          damageable.Damage(damage, this);
          HitEffect(other);
        }

        Release();
      }
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

    private void Awake()
    {
      hitLayersDefault = hitLayers;
    }

    private void OnEnable()
    {
      camera = Camera.main;
      hitLayers = hitLayersDefault;
    }

    private void FixedUpdate()
    {
      body.MovePosition(body.position + direction * (speed * Time.fixedDeltaTime));
      
      var pos = camera.WorldToViewportPoint(transform.position);

      if (pos.x < 0 || pos.x > 1 || pos.y < 0 || pos.y > 1)
      {
        Release();
      }
    }

    #endregion
  }
}