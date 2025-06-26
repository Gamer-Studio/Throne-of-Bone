using ToB.Entities.Buffs;
using UnityEngine;

namespace ToB.Worlds
{
  public class WaterObject : MonoBehaviour
  {
    [Tooltip("해당 물 오브젝트가 주는 피해량입니다.")] public int damage = 1;
    [Tooltip("피해를 주는 초 단위 주기입니다.")] public float delay = 1;
    [Tooltip("플레이어에게만 피해를 줄 지 여부입니다.")] public bool onlyPlayer = true;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
      if (other )
      {
        other.gameObject.SendMessage("WaterEnter", this, SendMessageOptions.DontRequireReceiver);
      }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
      if (other)
      {
        if (other.TryGetComponent<BuffController>(out var buffs))
        {
          buffs.Apply(Buff.Poison, new BuffInfo(1, 10), true);
        }
      }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
      if (other)
      {
        other.gameObject.SendMessage("WaterExit", this, SendMessageOptions.DontRequireReceiver);
      }
    }
  }
}