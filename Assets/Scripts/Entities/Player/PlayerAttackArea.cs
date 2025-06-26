using System;
using ToB.Entities;
using UnityEngine;

namespace ToB.Player
{
  public class PlayerAttackArea : MonoBehaviour
  {
    [SerializeField] private PlayerCharacter character;

    private void OnTriggerEnter2D(Collider2D other)
    {
      other.Damage(character.stat.atk, character);
    }
  }
}