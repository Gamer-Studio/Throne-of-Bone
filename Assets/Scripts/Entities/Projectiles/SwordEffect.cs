using System;
using Cysharp.Threading.Tasks.Triggers;
using NaughtyAttributes;
using UnityEngine;

namespace ToB.Entities.Projectiles
{
  public class SwordEffect : MonoBehaviour
  {
    [ReadOnly] private new Camera camera;
    [Label("피해량")] public float damage;
    [Label("발사 방향")] private Vector2 direction = Vector2.zero;
    [Label("속도")] public float speed = 1;
    [Label("넉백 세기")] public float knockBackForce = 15;
    
    public LayerMask hitLayers;
    
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private ParticleSystem ps;

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
      
      other.KnockBack(knockBackForce, direction);
      
      if (other.TryGetComponent<IDamageable>(out var damageable))
      {
        damageable.Damage(damage, this);
        gameObject.Release();
      }
    }

    private void OnEnable()
    {
      camera = Camera.main;
    }

    private void FixedUpdate()
    {
      body.MovePosition(body.position + direction * speed * Time.fixedDeltaTime);
      
      var pos = camera.WorldToViewportPoint(transform.position);

      if (pos.x < 0 || pos.x > 1 || pos.y < 0 || pos.y > 1)
      {
        gameObject.Release();
      }
    }

    #endregion
  }
}