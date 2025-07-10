using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace ToB.Player
{
  public partial class PlayerCharacter
  {
    #region State

    [Label("현재 방어 상태 지속 시간"), Foldout("Block State")] public float currentBlockTime = 0;

    #endregion
    
    #region Binding
    
    [Label("방어 파티클"), Foldout("Block State")] public GameObject blockParticleSystem;
    
    #endregion

    private void HandleBlockFocus()
    {
      
    }

    public UnityEvent<string, int> test;
  }
}