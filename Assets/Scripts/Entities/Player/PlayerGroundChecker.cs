using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace ToB.Player
{
  public class PlayerGroundChecker : MonoBehaviour
  {
    public UnityEvent onLanding = new();
    
    [field: SerializeField] public bool IsGround { get; private set; } = false;
    [SerializeField, ReadOnly] private List<GameObject> grounds = new();
    
    private void OnTriggerEnter2D(Collider2D other)
    {
      if (other.gameObject.CompareTag("Ground") && !grounds.Contains(other.gameObject))
      {
        grounds.Add(other.gameObject);
        
        if(!IsGround) onLanding?.Invoke();
          
        IsGround = true;
      }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
      if (grounds.Contains(other.gameObject))
      {
        grounds.Remove(other.gameObject);
        IsGround = grounds.Count > 0;
      }
    }
  }
}