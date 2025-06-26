using System;
using System.Collections;
using System.Collections.Generic;
using ToB.Entities;
using ToB.Player;
using UnityEngine;

namespace ToB.Worlds
{
  public class WaterObject : MonoBehaviour
  {
    [Tooltip("해당 물 오브젝트가 주는 피해량입니다.")] public int damage = 1;
    [Tooltip("피해를 주는 초 단위 주기입니다.")] public float delay = 1;
    [Tooltip("플레이어에게만 피해를 줄 지 여부입니다.")] public bool onlyPlayer = true;
    
    private readonly Dictionary<Collider2D, Coroutine> coroutines = new();
    
    private void OnTriggerEnter2D(Collider2D other)
    {
      if (other && !coroutines.ContainsKey(other) && other.TryGetComponent<IDamageable>(out var damageable))
      {
        coroutines.Add(other, StartCoroutine(Damage(damageable)));
        other.gameObject.SendMessage("WaterEnter", this, SendMessageOptions.DontRequireReceiver);
      }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
      if (other && coroutines.TryGetValue(other, out var coroutine))
      {
        StopCoroutine(coroutine);
        coroutines.Remove(other);
        other.gameObject.SendMessage("WaterExit", this, SendMessageOptions.DontRequireReceiver);
      }
    }

    private void OnDestroy()
    {
      if (coroutines != null)
      {
        foreach (var coroutine in coroutines.Values)
        {
          StopCoroutine(coroutine);
        }
        coroutines.Clear();
      }
    }

    private IEnumerator Damage(IDamageable comp)
    {
      for (;;)
      {
        yield return new WaitForSeconds(delay);
        
        if(!onlyPlayer || comp is PlayerCharacter)
          comp.Damage(damage);
      }
    }
  }
}