using ToB.Entities;
using UnityEngine;

namespace ToB.World.Structures
{
  public class DeathObject : Structure, IInteractable
  {
    public int gold;
    public int mana;
    
    public void Interact()
    {
      
    }

    public bool IsInteractable { get; set; }
    
    #region Unity Event

    private void OnTriggerEnter2D(Collider2D other)
    {
      
    }
    
    #endregion
  }
}