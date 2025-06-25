using System;
using UnityEngine;

namespace ToB.Player
{
  public class PlayerAttackArea : MonoBehaviour
  {
    [SerializeField] private PlayerCharacter character;

    private void OnTriggerEnter2D(Collider2D other)
    {
      Debug.Log(other.gameObject.name);
    }
  }
}