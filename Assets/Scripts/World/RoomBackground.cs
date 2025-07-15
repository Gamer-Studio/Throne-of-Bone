using UnityEngine;
using UnityEngine.Events;

namespace ToB.Worlds
{
  [AddComponentMenu("Stage/Room Background")]
  public class RoomBackground : MonoBehaviour
  {
    public UnityEvent onEnter = new();
    public UnityEvent onExit = new();

    public Collider2D backgroundCollider;
    
    public void OnTriggerEnter2D(Collider2D other)
    {
      if(other.CompareTag("Player")) onEnter?.Invoke();
    }
    
    public void OnTriggerExit2D(Collider2D other)
    {
      if(other.CompareTag("Player")) onExit?.Invoke();
    }
  }
}