using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace ToB.Player
{
  public partial class PlayerCharacter
  {
    #region State

    [Label("방어시 방어력 증가량"), Foldout("Block State")] public float additionalDef = 50;
    [Label("방어 게이지 회복까지 요구 딜레이"), Foldout("Block State")] public float blockEnergyRequireRegenDelay = 0.5f;
    [Label("방어 게이지 회복까지 현재 딜레이"), Foldout("Block State")] public float blockEnergyRegenDelay = 0;
    [Label("방어게이지 회복마다 걸리는 시간"), Foldout("Block State")] public float blockEnergyRegenTime = 0.1f;
    [Label("방어게이지 시간마다 회복량"), Foldout("Block State")] public float blockEnergyRegenAmount = 1;
    [Label("방어시 필요 방어 에너지"), Foldout("Block State")] public float requireBlockEnergy = 20;
    [Label("방어 유지시 필요 방어 에너지"), Foldout("Block State")] public float requireKeepBlockEnergy = 20;
    [Label("방어 쿨타임"), Foldout("Block State")] public float blockCoolTime = 1f;
    [Label("현재 방어 쿨타임"), Foldout("Block State"), SerializeField] private float currentBlockCoolTime = 0;
    [Label("패링 발동 제한 시간"), Foldout("Block State")] public float parryTimeLimit = 0.3f;
    [Label("방어 발동 제한 시간"), Tooltip("패링 시간 제외 방어 제한 시간입니다."), Foldout("Block State")] public float blockTimeLimit = 1.7f;
    [Label("패링 상태인지"), Foldout("Block State")] public bool isParrying = false;
    [Label("패링시 무적시간"), Foldout("Block State")] public float parryImmuneTime = 0.5f;
    [Label("패링시 멈추는 시간"), Foldout("Block State")] public float parryFreezeTime = 0.2f;
    [Label("패링했을 때 검기 회복량"), Foldout("Block State")] public int parryReward = 2;
    [Label("방어했을 때 검기 회복량"), Foldout("Block State")] public int blockReward = 1;

    //
    public bool IsBlocking => blockCoroutine != null;
    public bool IsBlockable => !IsBlocking && stat.BlockEnergy > requireBlockEnergy && !freezeBlockable && currentBlockCoolTime <= 0;
    [Foldout("Block State")] public bool freezeBlockable = false;
    
    #endregion
    
    #region Binding
    
    [Label("방어 파티클"), Foldout("Block State")] public GameObject shield;
    
    #endregion

    #region Feature
    
    private Coroutine blockCoroutine = null;

    public void Block(float damage, MonoBehaviour sender)
    {
      // 공격 방향 구하기
      var blockDir = (sender.transform.position - transform.position).normalized;
      var angle = Mathf.Atan2(blockDir.y, blockDir.x) * Mathf.Rad2Deg;
      Debug.Log(shield.transform.rotation.eulerAngles.z);
      // 공격 방향이 문제없을 시 방어 기능 구현
      if(isParrying)
      {
        // 패링
        AvailableRangedAttack += parryReward;
        if (immuneTime < parryImmuneTime) immuneTime = parryImmuneTime;
        
        Time.timeScale = 0;
        if(freezeTime < parryFreezeTime) freezeTime = parryFreezeTime;
      }
      else
      {
        // 일반 방어
        AvailableRangedAttack += blockReward;
        stat.Damage(damage);
      }
      
      animator.SetTrigger(TRIGGER_BLOCK);
      CancelBlock(isParrying);
    }
    
    /// <summary>
    /// 방어를 시작합니다.
    /// </summary>
    public void StartBlock()
    {
      if(!IsBlockable || IsMoving) return;

      stat.BlockEnergy -= requireBlockEnergy;
      blockCoroutine = StartCoroutine(BlockCoroutine());
    }
    
    /// <summary>
    /// 방어 상태를 취소합니다.
    /// </summary>
    public void CancelBlock(bool isParring = false)
    {
      if(blockCoroutine == null) return;
      
      StopCoroutine(blockCoroutine);
      EndBlock(isParring);
    }

    private void EndBlock(bool isParring = false)
    {
      stat.tempDef -= additionalDef;
      shield.SetActive(false);
      currentBlockCoolTime = isParring ? blockCoolTime : 0.1f;
      blockEnergyRegenDelay = blockEnergyRequireRegenDelay;
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

    private float blockEnergyCurrentRegenTime = 0;
    
    /// <summary>
    /// Update에서 사용됩니다.
    /// </summary>
    private void UpdateBlockResource()
    {
      if (currentBlockCoolTime > 0)
      {
        currentBlockCoolTime -= Time.deltaTime;
      }
      else currentBlockCoolTime = 0;

      if (!IsBlocking)
      {
        if (blockEnergyRegenDelay > 0)
        {
          blockEnergyRegenDelay -= Time.deltaTime;
        }
        else
        {
          blockEnergyRegenDelay = 0;
        
          blockEnergyCurrentRegenTime += Time.deltaTime;
          if(blockEnergyCurrentRegenTime >= blockEnergyRegenTime)
          {
            blockEnergyCurrentRegenTime = 0;
            stat.BlockEnergy += blockEnergyRegenAmount;

            if (freezeBlockable && stat.BlockEnergy >= 100)
              freezeBlockable = false;
          }
        }
      }
    }

    public void OnBlockEnergyChanged(float value)
    {
      if (value <= 0)
        freezeBlockable = true;
      else if (freezeBlockable && value >= 100)
        freezeBlockable = false;
    }

    private IEnumerator BlockCoroutine()
    {
      isParrying = true;
      shield.SetActive(true);
      stat.tempDef += additionalDef;
      
      yield return new WaitForSeconds(parryTimeLimit);
      isParrying = false;

      for (float blockTime = 0, rkbe = requireKeepBlockEnergy * (Time.fixedDeltaTime / blockTimeLimit); blockTime < blockTimeLimit; blockTime += Time.fixedDeltaTime)
      {
        stat.BlockEnergy -= rkbe;
        if(stat.BlockEnergy <= 0) CancelBlock();
        yield return new WaitForFixedUpdate();
      }
      
      EndBlock();
    }
    
    #endregion
  }
}