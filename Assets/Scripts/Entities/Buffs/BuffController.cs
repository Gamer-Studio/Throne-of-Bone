using UnityEngine;

namespace ToB.Entities.Buffs
{
  public class BuffController : MonoBehaviour
  {
    [SerializeField] private SerializableDictionary<Buff, BuffInfo> buffs = new();
    
    /// <summary>
    /// 버프를 추가합니다.
    /// </summary>
    /// <param name="buff">추가할 버프의 종류입니다.</param>
    /// <param name="info">추가할 버프의 업데이트되는 정보입니다.</param>
    /// <param name="force">true일 경우 버프가 존재할 경우 info를 덮어씌웁니다.</param>
    public void Apply(Buff buff, BuffInfo info, bool force = false)
    {
      if (!buffs.ContainsKey(buff))
      {
        buff.Apply(gameObject, info);
      }
      else if (!force) return;
      
      buffs[buff] = info;
    }
    
    /// <summary>
    /// 버프를 강제로 제거합니다.
    /// </summary>
    /// <param name="buff">제거할 버프의 타입입니다.</param>
    public void Remove(Buff buff)
    {
      buff.Remove(gameObject);
      buffs.Remove(buff);
    }

    private void FixedUpdate()
    {
      foreach (var buff in buffs)
      {
        var info = buff.Value;
        
        info.duration -= Time.fixedDeltaTime;
        info.delayTime += Time.fixedDeltaTime;
        
        if (info.delayTime >= info.delay)
        {
          buff.Key.Effect(gameObject, info);
          info.delayTime = 0;
        }
        
        if (info.duration <= 0) Remove(buff.Key);
      }
    }
  }
}