using UnityEngine;

namespace ToB.Entities
{
  public interface IKnockBackable
  {
    /// <summary>
    /// 대상에게 넉백을 가합니다.
    /// </summary>
    /// <param name="value">넉백 세기입니다.</param>
    /// <param name="direction">넉백을 가하는 방향입니다.</param>
    void KnockBack(float value, Vector2 direction);
    
    /// <summary>
    /// 오브젝트 위치를 기반으로 대상에게 넉백을 가합니다.
    /// </summary>
    /// <param name="value">넉백 세기입니다.</param>
    /// <param name="sender">넉백을 가하는 주체입니다.</param>
		void KnockBack(float value, GameObject sender);
  }

  public static class IKnockBackableExtension
  {
    /// <summary>
    /// 오브젝트가 넉백이 가능할 경우 넉백을 하는 확장메서드입니다.
    /// </summary>
    /// <param name="target">넉백 대상입니다.</param>
    /// <param name="value">넉백 세기입니다.</param>
    /// <param name="direction">넉백 방향입니다.</param>
    public static void KnockBack(this GameObject target, float value, Vector2 direction)
    {
      if (target && target.TryGetComponent<IKnockBackable>(out var knockBackable))
      {
        knockBackable.KnockBack(value, direction);
      }
    }
    
    /// <summary>
    /// 오브젝트가 넉백이 가능할 경우 넉백을 하는 확장메서드입니다.
    /// </summary>
    /// <param name="target">넉백 대상입니다.</param>
    /// <param name="value">넉백 세기입니다.</param>
    /// <param name="sender">넉백을 가하는 주체입니다.</param>
    public static void KnockBack(this GameObject target, float value, GameObject sender)
    {
      if (target && target.TryGetComponent<IKnockBackable>(out var knockBackable))
      {
        knockBackable.KnockBack(value, sender);
      }
    }
    
    /// <summary>
    /// 오브젝트가 넉백이 가능할 경우 넉백을 하는 확장메서드입니다.
    /// </summary>
    /// <param name="coll">넉백 대상입니다.</param>
    /// <param name="value">넉백 세기입니다.</param>
    /// <param name="direction">넉백 방향입니다.</param>
    public static void KnockBack(this Collider2D coll, float value, Vector2 direction)
    {
      if (coll && coll.TryGetComponent<IKnockBackable>(out var knockBackable))
      {
        knockBackable.KnockBack(value, direction);
      }
    }

    /// <summary>
    /// 오브젝트가 넉백이 가능할 경우 넉백을 하는 확장메서드입니다.
    /// </summary>
    /// <param name="coll">넉백 대상입니다.</param>
    /// <param name="value">넉백 세기입니다.</param>
    /// <param name="sender">넉백을 가하는 주체입니다.</param>
    public static void KnockBack(this Collider2D coll, float value, GameObject sender)
    {
      if (coll && coll.TryGetComponent<IKnockBackable>(out var knockBackable))
      {
        knockBackable.KnockBack(value, sender);
      }
    }
  }
}