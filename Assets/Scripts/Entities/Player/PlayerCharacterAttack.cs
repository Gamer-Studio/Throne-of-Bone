using System;
using System.Collections;
using NaughtyAttributes;
using ToB.Entities;
using ToB.Entities.Projectiles;
using UnityEngine;

namespace ToB.Player
{
  public partial class PlayerCharacter
  {
    #region State
    
    [Label("공격 모션 재생 여부"), Foldout("Attack State")] public bool isAttacking = false; 
    [Label("근접 공격 딜레이"), Tooltip("0번 인덱스는 모션 리셋 시간 / 나머지는 모션의 대기 시간입니다."), Foldout("Attack State")] public float[] meleeAttackDelay = {1, 0.3f, 0.3f, 1f};
    [Label("근접 공격 피해 계수"), Tooltip("기본 캐릭터 공격력에 비례한 모션당 피해 계수입니다."), Foldout("Attack State")] public float[] meleeAttackDamageMultiplier = {1, 1, 2};
    
    [Label("최대 원거리 공격 횟수"), Tooltip("원거리 공격의 충전되는 최대 횟수입니다."), Foldout("Attack State")] public int maxRangedAttack = 3;
    [Label("원거리 공격 스택"), Foldout("Attack State"), SerializeField, ReadOnly] private int availableRangedAttack = 5;
    [Label("원거리 공격 스택 재생 시간(초)"), Foldout("Attack State")] public float rangedAttackRegenTime = 1;
    [Label("원거리 공격 딜레이"), Foldout("Attack State")] public float rangedAttackDelay = 0.2f;
    
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

    #region Binding

    [Label("검기 프리팹"), SerializeField] private GameObject swordEffect;

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
      
      if(meleeAttackCoroutine != null) StopCoroutine(meleeAttackCoroutine);
      meleeAttackCoroutine = StartCoroutine(MeleeAttackWaiter(direction, meleeAttackDelay[prevMeleeAttackMotion + 1]));
      animator.SetInteger(INT_ATTACK_MOTION, prevMeleeAttackMotion);
      prevMeleeAttackMotion = prevMeleeAttackMotion == 2 ? 0 : prevMeleeAttackMotion + 1;
      animator.SetTrigger(TRIGGER_ATTACK);

      // 원거리 공격 구현
      if (!isMelee && AvailableRangedAttack > 0)
      {
        var eff = swordEffect.Pooling().GetComponent<SwordEffect>();

        eff.transform.position = transform.position;
        eff.Direction = direction;
        eff.damage = stat.atk / 2;
        
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