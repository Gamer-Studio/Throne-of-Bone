using System;
using System.Collections;
using NaughtyAttributes;
using ToB.Entities;
using ToB.Entities.Projectiles;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace ToB.Player
{
  public partial class PlayerCharacter
  {
    #region State
    
    [Label("공격 모션 재생 여부"), Foldout("Attack State")] public bool isAttacking = false; 
    [Label("공격 딜레이"), Tooltip("0번 인덱스는 모션 리셋 시간 / 1, 2, 3번은 근접 공격 대기시간, 4번은 원거리 대기 시간입니다."), Foldout("Attack State")] 
    public float[] attackDelay = {1, 0.3f, 0.3f, 0.1f};
    [Label("근접 공격 피해 계수"), Tooltip("기본 캐릭터 공격력에 비례한 모션당 피해 계수입니다."), Foldout("Attack State")] public float[] attackDamageMultiplier = {1, 1, 2, 0.5f};
    [Label("근접 공격 대기 시간")] private float meleeAttackDelay = 0;
    [Label("최대 원거리 공격 횟수"), Tooltip("원거리 공격의 충전되는 최대 횟수입니다."), Foldout("Attack State")] public int maxRangedAttack = 3;
    [Label("원거리 공격 스택"), Foldout("Attack State"), SerializeField, ReadOnly] private int availableRangedAttack = 5;
    [Label("원거리 공격 스택 재생 시간(초)"), Foldout("Attack State")] public float rangedAttackRegenTime = 1;
    [Label("원거리 공격 딜레이"), Foldout("Attack State")] public float rangedAttackDelay = 0.2f;
    [Label("원거리 공격 발사 대기시간"), Foldout("Attack State")] public float shootDelay = 0.1f;
    
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

    public float MeleeAttackDelay
    {
      get => meleeAttackDelay;
      set
      {
        meleeAttackDelay = value;
        attackGaugeBar.Value = value;
      }
    }
    
    #endregion

    #region Binding

    [Label("검기 프리팹"), Foldout("Attack State"), SerializeField] private GameObject swordEffect;

    #endregion
    
    /// <summary>
    /// 원거리 공격 가능 횟수가 변경될 시 호출되며, 매개변수로 현재 원거리 공격가능 횟수를 넘겨줍니다.
    /// </summary>
    [Foldout("Attack State")] public UnityEvent<int> OnRangedAttackStackChange = new();
    
    private bool IsAttackMotion => animator.GetCurrentAnimatorStateInfo(0).IsName("Slash0") ||
                                   animator.GetCurrentAnimatorStateInfo(0).IsName("Slash1") ||
                                   animator.GetCurrentAnimatorStateInfo(0).IsName("Slash2");
    private int prevAttackMotion = 0;
    
    private Coroutine attackCoroutine = null;
    private Coroutine rangedCoroutine = null;

    private void InitAttack()
    {
      attackGaugeBar.max = 1;
      attackGaugeBar.Value = 0;
    }
    
    private static readonly Vector2[] directions = { Vector2.right, Vector2.left, Vector2.down };

    
    /// <summary>
    /// direction 방향으로 공격합니다.
    /// isMelee를 false로 하여 원거리 공격을 할 수 있습니다.
    /// </summary>
    /// <param name="direction">공격 방향입니다.</param>
    /// <param name="isMelee">근거리/원거리 공격 방향입니다.</param>
    public void Attack(Vector2 direction, bool isMelee = true)
    {
      var minDist = float.MaxValue;
      var closest = Vector2.zero;
      
      foreach (var d in directions)
      {
        var dist = Vector2.Distance(direction, d);
        if (dist < minDist)
        {
          minDist = dist;
          closest = d;
        }
      }
      if (closest == Vector2.down) animator.SetInteger(INT_ATTACK_MOTION, 3);
      
      if (isAttacking || AvailableRangedAttack <= 0) return;
      if(IsDashing) CancelDash();
      if(attackCoroutine != null) StopCoroutine(attackCoroutine);

      isAttacking = true;
      
      if (closest != Vector2.down)
      {
        var pam = isMelee ? prevAttackMotion : 3;
        attackCoroutine = StartCoroutine(AttackWaiter(direction, attackDelay[pam + 1]));
        animator.SetInteger(INT_ATTACK_MOTION, pam);
      
        prevAttackMotion = prevAttackMotion == 2 ? 0 : prevAttackMotion + 1;
      }
      else 
        attackCoroutine = StartCoroutine(AttackWaiter(direction, 0.1f));
      
      animator.SetTrigger(TRIGGER_ATTACK);

      // 원거리 공격 구현
      if (!isMelee)
      {
        StartCoroutine(Shoot(direction));
      }
    }
    
    /// <summary>
    /// 이벤트 트리거에요. 호출하지 말아주세요!
    /// </summary>
    public void AttackEnd()
    {
      transform.eulerAngles = new Vector3(0, MoveDirection == PlayerMoveDirection.Left ? 180 : 0, 0);
    }

    private IEnumerator Shoot(Vector2 direction)
    {
      yield return new WaitForSeconds(shootDelay);
      var eff = swordEffect.Pooling().GetComponent<SwordEffect>();

      eff.transform.position = transform.position;
      eff.Direction = direction;
      eff.damage = stat.atk / 2;
        
      // 탄환? 관리
      AvailableRangedAttack--;
      rangedCoroutine ??= StartCoroutine(RegenRangedAttack());
    }

    private void SetMeleeAttackDelay(float value)
    {
      MeleeAttackDelay = value;
      attackGaugeBar.max = value;
    }
    
    private IEnumerator AttackWaiter(Vector2 direction, float waitTime)
    {
      transform.eulerAngles = new Vector3(0, direction.x < 0 ? 180 : 0, 0);
      
      var prevDamage = stat.atk;
      stat.atk *= attackDamageMultiplier[prevAttackMotion];
      SetMeleeAttackDelay(waitTime);
      yield return new WaitForSeconds(waitTime);
      isAttacking = false;
      stat.atk = prevDamage;

      yield return new WaitForSeconds(attackDelay[0]);
      prevAttackMotion = 0;

      attackCoroutine = null;
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