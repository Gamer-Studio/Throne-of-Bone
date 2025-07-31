using System;
using System.Collections;
using NaughtyAttributes;
using ToB.Core;
using ToB.Entities.Interface;
using ToB.Entities.Projectiles;
using ToB.Entities.Skills;
using ToB.Utils;
using ToB.Worlds;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using AudioType = ToB.Core.AudioType;

namespace ToB.Player
{
  public partial class PlayerCharacter : IAttacker
  {
    #region State

    private const string AttackState = "Attack State";
    
    [Label("공격 모션 재생 여부"), Foldout(AttackState)] public bool isAttacking = false; 
    [Label("공격 딜레이"), Tooltip("0번 인덱스는 모션 리셋 시간 / 1, 2, 3번은 근접 공격 대기시간, 4번은 원거리 대기 시간입니다."), Foldout(AttackState)] 
    public float[] attackDelay = {1, 0.3f, 0.3f, 0.1f};
    [Label("근접 공격 피해 계수"), Tooltip("기본 캐릭터 공격력에 비례한 모션당 피해 계수입니다."), Foldout(AttackState)] public float[] attackDamageMultiplier = {1, 1, 2, 0.5f};
    [Label("근접 공격 대기 시간"), Foldout(AttackState), SerializeField] private float meleeAttackDelay = 0;
    [Label("최대 원거리 공격 횟수"), Tooltip("원거리 공격의 충전되는 최대 횟수입니다."), Foldout(AttackState)] public int maxRangedAttack = 3;
    [Label("검기 스택"), Foldout(AttackState), SerializeField] private int availableRangedAttack = 3;
    [Label("검기 재생 시간(초)"), Foldout(AttackState)] public float rangedAttackRegenTime = 1;
    [Label("검기 초당 회복량"), Foldout(AttackState)] public int rangedAttackRegenAmount = 0;
    [Label("검기 발사 대기시간"), Foldout(AttackState)] public float shootDelay = 0.1f;
    [Label("패링 가능 레이어"), Foldout(AttackState)] public LayerMask parryableLayer;

    /// <summary>
    /// 적이 플레이어의 공격을 막을 수 있는지 여부입니다.
    /// </summary>
    public bool Blockable => true;

    /// <summary>
    /// 플레이어의 공격에 피격됬을 때 이펙트를 발생시키는지 여부입니다.
    /// </summary>
    public bool Effectable => true;

    public Vector3 Position => transform.position;
    public Team Team => Team.None;

    // 현재 원거리 공격 가능 횟수입니다.
    public int AvailableRangedAttack
    {
      get => availableRangedAttack;
      set
      {
        var input = Math.Min(maxRangedAttack + BattleSkillManager.instance.BSStats.RangeAtkStack, Math.Max(value, 0));

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

    [Label("검기 프리팹 주소"), Foldout("Attack State"), SerializeField] private AssetReference swordEffect;
    [Foldout("Attack State"), SerializeField] private ParticleSystem rangeAttackGlowEffect;

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

    [NonSerialized] public bool bottomJumpAvailable = false;

    /// <summary>
    /// direction 방향으로 공격합니다.
    /// isMelee를 false로 하여 원거리 공격을 할 수 있습니다.
    /// </summary>
    /// <param name="direction">공격 방향입니다.</param>
    /// <param name="isMelee">근거리/원거리 공격 방향입니다.</param>
    /// <param name="bottomAttack"></param>
    public void Attack(Vector2 direction, bool isMelee = true, bool bottomAttack = false)
    {
      if (bottomAttack) animator.SetInteger(INT_ATTACK_MOTION, 4);
      
      if (isAttacking) return;

      var attackCancel = true;

      if (isMelee)
      {
        // 근거리 공격
        CancelBlock();
        if(IsDashing) CancelDash();
        if(attackCoroutine != null) StopCoroutine(attackCoroutine);

        isAttacking = true;
      
        // 아래공격
        if(bottomAttack && IsFlight)
        {
          bottomJumpAvailable = true;
          attackCoroutine = StartCoroutine(AttackWaiter(direction, 0.1f));
          AudioManager.Play("fntgm_blade_whoosh_light_02", AudioType.Effect); // 아래공격 사운드
        }
        else
        {
          var pam = prevAttackMotion;
          attackCoroutine = StartCoroutine(AttackWaiter(direction, attackDelay[pam + 1]));
          animator.SetInteger(INT_ATTACK_MOTION, pam);
          if (prevAttackMotion == 0) AudioManager.Play("fntgm_blade_whoosh_light_02",AudioType.Effect); // 1타
          else if (prevAttackMotion == 1) AudioManager.Play("fntgm_blade_whoosh_med_03",AudioType.Effect); // 2타
          else if (prevAttackMotion == 2) AudioManager.Play("fntgm_blade_whoosh_heavy_03",AudioType.Effect); // 3타
      
          prevAttackMotion = prevAttackMotion == 2 ? 0 : prevAttackMotion + 1;
        }
        
        attackCancel = false;
      }
      else if (AvailableRangedAttack > 0)
      {
        // 원거리 공격
        CancelBlock();
        if(IsDashing) CancelDash();
        if(attackCoroutine != null) StopCoroutine(attackCoroutine);
        
        isAttacking = true;
      
        if(bottomAttack && IsFlight)
        {
          attackCoroutine = StartCoroutine(AttackWaiter(direction, 0.1f));
        }
        else
        {
          const int pam = 3;
          attackCoroutine = StartCoroutine(AttackWaiter(direction, attackDelay[pam + 1]));
          animator.SetInteger(INT_ATTACK_MOTION, pam);
      
          prevAttackMotion = prevAttackMotion == 2 ? 0 : prevAttackMotion + 1;
        }
        
        attackCancel = false;
        StartCoroutine(Shoot(direction));
      }

      if (!attackCancel)
      {
        attackDirection = direction.x > 0 ? PlayerMoveDirection.Right : PlayerMoveDirection.Left;
        animator.SetTrigger(TRIGGER_ATTACK);
      }
    }
    
    /// <summary>
    /// 이벤트 트리거에요. 호출하지 말아주세요!
    /// </summary>
    public void AttackEnd()
    {
      bottomJumpAvailable = false;
      // transform.eulerAngles = new Vector3(0, MoveDirection == PlayerMoveDirection.Left ? 180 : 0, 0);
    }

    private IEnumerator Shoot(Vector2 direction)
    {
      yield return new WaitForSeconds(shootDelay);
      var effect = (SwordEffect)swordEffect.Pooling();
      effect.ClearEffect();

      effect.transform.position = transform.position;
      effect.Direction = direction;
      effect.damage = stat.atk / 2;
      effect.launcher = gameObject;
      
      AudioManager.Play("fntgm_arrow_whoosh", AudioType.Effect);
        
      if(rangeAttackGlowEffect.isPlaying) rangeAttackGlowEffect.Stop();
      rangeAttackGlowEffect.Play();
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
      
      var prevDamage = stat.atk.BaseValue;
      if (prevAttackMotion < 3) stat.atk *= attackDamageMultiplier[prevAttackMotion];
      else if (prevAttackMotion == 3) stat.atk *= (attackDamageMultiplier[prevAttackMotion] + BattleSkillManager.instance.BSStats.RangeAtkDmgMultiplier);
      SetMeleeAttackDelay(waitTime);
      yield return new WaitForSeconds(waitTime);
      isAttacking = false;
      stat.atk.BaseValue = prevDamage;

      yield return new WaitForSeconds(attackDelay[0]);
      prevAttackMotion = 0;

      attackCoroutine = null;
    }

    private IEnumerator RegenRangedAttack()
    {
      for (;availableRangedAttack < maxRangedAttack + BattleSkillManager.instance.BSStats.RangeAtkStack;)
      {
        yield return new WaitForSeconds(rangedAttackRegenTime);
        AvailableRangedAttack += rangedAttackRegenAmount;
      }

      rangedCoroutine = null;
    }
  }
}