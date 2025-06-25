using UnityEngine;

namespace ToB.Entities
{
  public interface IDamageable
  {
    /// <param name="damage">대상에게 가하는 피해량입니다.</param>
		void Damage(float damage);
  }

  public static class IDamageableExtensions
  {
    /// <summary>
    /// 오브젝트에 바로 피해를 줄 수 있게해주는 확장 메서드입니다.
    /// </summary>
    /// <param name="obj">IDamageable 컴포넌트가 붙어있는 오브젝트입니다.</param>
    /// <param name="damage">대상에게 가하는 피해량입니다.</param>
    public static void Damage(this GameObject obj, float damage)
    {
      if (obj && obj.TryGetComponent(out IDamageable damageable))
      {
        damageable.Damage(damage);
      }
    }

    /// <summary>
    /// 이벤트를 통해 바로 피해를 줄 수 있게해주는 확장 메서드입니다.
    /// </summary>
    /// <param name="coll">IDamageable 컴포넌트가 붙어있는 오브젝트입니다.</param>
    /// <param name="damage">대상에게 가하는 피해량입니다.</param>
    public static void Damage(this Collider2D coll, float damage)
    {
      if (coll && coll.TryGetComponent(out IDamageable damageable))
      {
        damageable.Damage(damage);
      }
    }
  }
}