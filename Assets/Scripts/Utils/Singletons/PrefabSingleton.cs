using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ToB.Utils.Singletons
{
  public abstract class PrefabSingleton<T> : MonoBehaviour where T : MonoBehaviour
  {
    private AssetReferenceGameObject reference;
    private static T instance;

    /// <summary>
    /// get 시 어드레서블에서 해당 클래스명 주소로 된 프리팹을 찾아와서 인스턴스합니다.
    /// </summary>
    public static T Instance
    {
      get
      {
        if (instance != null) return instance;
        
        var reference = new AssetReferenceGameObject(instance.GetType().Name);
        var obj = reference.InstantiateAsync().WaitForCompletion();
        obj.GetComponent<PrefabSingleton<T>>().reference = reference;
        
        // 생성된 게임 오브젝트에서 해당 컴포넌트를 instance에 저장한다.
        instance = obj.GetComponent<T>();

        return instance;
      }
    }

    private void OnDestroy()
    {
      reference.ReleaseInstance(gameObject);
      reference.ReleaseAsset();
      instance = null;
    }
  }
}