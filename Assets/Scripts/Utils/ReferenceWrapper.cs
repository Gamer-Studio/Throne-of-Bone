using System;
using UnityEngine.AddressableAssets;

namespace ToB.Utils
{
  [Serializable]
  public class ReferenceWrapper
  {
    /// <summary>
    /// 어드레서블 참조입니다.
    /// </summary>
    public AssetReference assetReference;
    
    /// <summary>
    /// 읽기전용 주소입니다.
    /// </summary>
    public string path;

    public ReferenceWrapper() { }

    public ReferenceWrapper(AssetReference assetReference, string path)
    {
      this.assetReference = assetReference;
      this.path = path;
    }

    public override string ToString() => path;
  }
}