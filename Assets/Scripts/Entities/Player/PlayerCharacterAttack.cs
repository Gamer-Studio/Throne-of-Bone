using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace ToB.Player
{
  public partial class PlayerCharacter
  {
    #region State
    
    [Header("Attack State")] 
    [Tooltip("공격 모션 재생 여부")] public bool isAttacking = false; 
    /// <summary>
    /// 0번 인덱스는 모션 리셋 시간입니다.
    /// </summary>
    [Tooltip("근접 공격 딜레이")] public float[] meleeAttackDelay = {1, 0.3f, 0.3f, 1f};
    [Tooltip("근접 공격 피해 계수")] public float[] meleeAttackDamageMultiplier = {1, 1, 2};
    [Tooltip("최대 원거리 공격 스택")] public int maxRangedAttack = 3;
    [Tooltip("원거리 공격 스택"), SerializeField, ReadOnly] private int availableRangedAttack = 5;
    [Tooltip("원거리 공격 스택 재생 시간(초)")] public float rangedAttackRegenTime = 1;
    [Tooltip("원거리 공격 딜레이")] public float rangedAttackDelay = 0.2f;
    
    // 현재 원거리 공격 가능 횟수입니다.
    public int AvailableRangedAttack
    {
      get => availableRangedAttack;
      set
      {
        var input = Math.Min(maxRangedAttack, Math.Max(value, 0));

        if (input != availableRangedAttack)
        {
          availableRangedAttack = input;
          OnRangedAttackStackChange.Invoke(availableRangedAttack);
        }
      }
    }
    
    #endregion

    private bool IsAttackMotion => animator.GetCurrentAnimatorStateInfo(0).IsName("Slash0") ||
                                   animator.GetCurrentAnimatorStateInfo(0).IsName("Slash1") ||
                                   animator.GetCurrentAnimatorStateInfo(0).IsName("Slash2");
    private int prevMeleeAttackMotion = 0;
    
    private Coroutine meleeAttackCoroutine = null;
    private Coroutine rangedCoroutine = null;
    
    /// <summary>
    /// direction 방향으로 공격합니다.
    /// isMelee를 false로 하여 원거리 공격을 할 수 있습니다.
    /// </summary>
    /// <param name="direction">공격 방향입니다.</param>
    /// <param name="isMelee">근거리/원거리 공격 방향입니다.</param>
    public void Attack(Vector2 direction, bool isMelee = true)
    {
      if (isAttacking) return;
      // 애니메이션 구현
      isAttacking = true;

      if(IsDashing) CancelDash();
      
      if (isMelee)
      {
        if(meleeAttackCoroutine != null) StopCoroutine(meleeAttackCoroutine);
        meleeAttackCoroutine = StartCoroutine(MeleeAttackWaiter(direction, meleeAttackDelay[prevMeleeAttackMotion + 1]));
      }
      
      animator.SetInteger(INT_ATTACK_MOTION, prevMeleeAttackMotion);
      prevMeleeAttackMotion = prevMeleeAttackMotion == 2 ? 0 : prevMeleeAttackMotion + 1;
      animator.SetTrigger(TRIGGER_ATTACK);
      
      
      if (direction.x > 0)
      {
          
      }
      else
      {
          
      }

      // 원거리 공격 구현
      if (!isMelee && AvailableRangedAttack > 0)
      {
        // 탄환? 관리
        AvailableRangedAttack--;
        if (rangedCoroutine == null) 
          rangedCoroutine = StartCoroutine(RegenRangedAttack());
      }
    }

    /// <summary>
    /// 이벤트 트리거에요. 호출하지 말아주세요!
    /// </summary>
    public void AttackEnd()
    {
      transform.eulerAngles = new Vector3(0, MoveDirection == PlayerMoveDirection.Left ? 180 : 0, 0);
    }

    private IEnumerator MeleeAttackWaiter(Vector2 direction, float waitTime)
    {
      transform.eulerAngles = new Vector3(0, direction.x < 0 ? 180 : 0, 0);
      
      var prevDamage = stat.atk;
      stat.atk *= meleeAttackDamageMultiplier[prevMeleeAttackMotion];
      yield return new WaitForSeconds(waitTime);
      isAttacking = false;
      stat.atk = prevDamage;

      yield return new WaitForSeconds(meleeAttackDelay[0]);
      prevMeleeAttackMotion = 0;

      meleeAttackCoroutine = null;
    }

    private IEnumerator RegenRangedAttack()
    {
      for (;availableRangedAttack < maxRangedAttack;)
      {
        yield return new WaitForSeconds(rangedAttackRegenTime);
        AvailableRangedAttack++;
      }

      rangedCoroutine = null;
    }
  }
}