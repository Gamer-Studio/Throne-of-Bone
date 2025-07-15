using UnityEngine;
using UnityEngine.Pool;

namespace ToB.Utils
{
  public class PooledObject : MonoBehaviour
  {
    public ObjectPool<PooledObject> pool = null;

    public virtual void Release()
    {
      if (pool != null)
      {
        pool.Release(this);
      }
      else Destroy(gameObject);
    }

    public PooledObject Duplicate() => pool != null ? pool.Get() : gameObject.GetComponent<PooledObject>();

    public static implicit operator GameObject(PooledObject obj) => obj.gameObject;
  }
}