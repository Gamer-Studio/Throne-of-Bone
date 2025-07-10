using System.Collections;
using NaughtyAttributes;
using ToB.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace ToB.Player
{
  public partial class PlayerCharacter
  {
    #region State

    [Label("방어시 방어력 증가량"), Foldout("Block State")] public float additionalDef = 50;
    [Label("현재 방어 상태 지속 시간"), Foldout("Block State")] public float currentBlockTime = 0;
    [Label("패링 발동 제한 시간"), Foldout("Block State")] public float parryTimeLimit = 0.3f;

    #endregion
    
    #region Binding
    
    [Label("방어 파티클"), Foldout("Block State")] public GameObject shield;
    
    #endregion

    #region Feature
    
    private Coroutine blockCoroutine = null;
    
    /// <summary>
    /// 방어를 시작합니다.
    /// </summary>
    public void StartBlock()
    {
      if(blockCoroutine != null) return;

      blockCoroutine = StartCoroutine(BlockCoroutine());
    }
    
    public void CancelBlock()
    {
      if(blockCoroutine == null) return;
      
      StopCoroutine(blockCoroutine);
      EndBlock();
    }

    private void EndBlock()
    {
      stat.tempDef -= additionalDef;
      shield.SetActive(false);
      blockCoroutine = null;
    }
    
    /// <summary>
    /// 방어막의 방향으 선택할 수 있습니다.\n
    /// 방향은 (1, 1) ~ (-1, -1) 이내로 입력해주세요.
    /// </summary>
    public void SetBlockFocus(Vector3 direction)
    {
      shield.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, direction));
    }

    private void RegenBlockEnergy()
    {
      
    }

    private IEnumerator BlockCoroutine()
    {
      shield.SetActive(true);
      stat.tempDef += additionalDef;
      
      yield return new WaitForSeconds(0.1f);
      // EndBlock();
    }
    
    #endregion
  }
}