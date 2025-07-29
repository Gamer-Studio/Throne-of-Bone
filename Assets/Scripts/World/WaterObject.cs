using System.Collections.Generic;
using NaughtyAttributes;
using ToB.Entities.Buffs;
using ToB.Player;
using UnityEngine;

namespace ToB.Worlds
{
  [AddComponentMenu("Stage/Object/Water")]
  public class WaterObject : MonoBehaviour
  {
    [Label("플레이어만 적용"), Tooltip("플레이어에게만 적용하는지 여부입니다.")] public bool onlyPlayer = true;
    [Label("적용시킬 버프 정보")] public SerializableDictionary<Buff, BuffInfo> buffs = new();
    
    private void OnTriggerEnter2D(Collider2D other)
    {
      if (other)
      {
        other.gameObject.SendMessage("WaterEnter", this, SendMessageOptions.DontRequireReceiver);
      }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
      if (other && (!onlyPlayer || other.TryGetComponent<PlayerCharacter>(out _)))
      {
        if (other.TryGetComponent<BuffController>(out var controller))
        {
          foreach (var pair in buffs)
          {
            controller.Apply(pair.Key, pair.Value, true);
          }
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