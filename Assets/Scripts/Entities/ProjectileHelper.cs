using UnityEngine;

namespace ToB.Entities
{
  public static class ProjectileHelper
  {
    private static ProjectileManager Manager => ProjectileManager.Instance;
    
    /// <summary>
    /// 프리팹의 오브젝트 풀을 없으면 생성해서 풀링합니다.
    /// </summary>
    /// <param name="prefab">풀링할 프리팹입니다.</param>
    /// <returns>풀링된 오브젝트입니다.</returns>
    public static GameObject Pooling(this GameObject prefab)
    {
      if (!Manager.IsRegistered(prefab.name))
        Manager.Register(prefab.name, prefab);
      
      return Manager.Get(prefab.name);
    }
    
    /// <summary>
    /// 오브젝트를 해제하거나 파괴합니다.
    /// </summary>
    /// <param name="object">해제할 오브젝트입니다.</param>
    public static void Release(this GameObject @object) => Manager.Release(@object);
  }
}