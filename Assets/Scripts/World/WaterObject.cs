using System;
using UnityEngine;

namespace ToB.Worlds
{
  public class WaterObject : MonoBehaviour
  {
    public int damage = 1;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
      if (other.gameObject)
      {
        other.gameObject.SendMessage("WaterEnter", damage, SendMessageOptions.DontRequireReceiver);
      }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
      if (other.gameObject)
      {
        other.gameObject.SendMessage("WaterExit", SendMessageOptions.DontRequireReceiver);
      }
    }
  }
}