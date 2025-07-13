using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using ToB.Entities;
using ToB.Utils;
using ToB.Utils.UI;
using ToB.Worlds;
using UnityEngine;
using UnityEngine.Events;

namespace ToB.Player
{
  public partial class PlayerCharacter : MonoBehaviour, IDamageable, IKnockBackable
  {
    private static readonly int INT_STATE = Animator.StringToHash("State");
    private static readonly int BOOL_IMMUNE = Animator.StringToHash("Immune");
    private static readonly int BOOL_IS_FLIGHT = Animator.StringToHash("IsFlight");
    private static readonly int BOOL_FALLING = Animator.StringToHash("Falling");
    private static readonly int TRIGGER_FALL = Animator.StringToHash("Fall");
    private static readonly int TRIGGER_JUMP = Animator.StringToHash("Jump");
    private static readonly int TRIGGER_BLOCK = Animator.StringToHash("Block");
    private static readonly int BOOL_CLIMB = Animator.StringToHash("Climb");
    private static readonly int TRIGGER_DASH = Animator.StringToHash("Dash");
    private static readonly int INT_DASH_STATE = Animator.StringToHash("DashState");
    private static readonly int TRIGGER_ATTACK = Animator.StringToHash("Attack");
    private static readonly int INT_ATTACK_MOTION = Animator.StringToHash("AttackMotion");

    /// <returns>현재 활성화된 Player 태그가 붙은 오브젝트의 캐릭터를 찾아옵니다.</returns>
    public static PlayerCharacter GetInstance()
    {
      foreach (var obj in GameObject.FindGameObjectsWithTag("Player"))
      {
        if (obj.TryGetComponent<PlayerCharacter>(out var comp) && obj.gameObject.activeSelf)
        {
          return comp;
        }
      }
      Debug.Log("플레이어 인스턴스 반환 실패");
      return null;
    }
    
    #region State

    [Label("조작 가능"), Foldout("State")] public bool isControlable = true;
    [Label("애니메이션 상태"), Foldout("State"), SerializeField, GetSet(nameof(AnimationState))] protected PlayerAnimationState animationState = PlayerAnimationState.Idle;
    [Label("이동속도"), Foldout("State")] public float moveSpeed = 2;
    [Label("최대 이동 속도"), Foldout("State")] public float maxMoveSpeed = 12;
    [Label("좌/우 마찰력 보정값"), Foldout("State")] public float moveResistanceForce = 1;
    [Label("벽타기 하강 속도"), Foldout("State")] public float wallEnduringSpeed = -1;
    [Label("이동방향 (좌/우)"), Foldout("State"), SerializeField, ReadOnly] private PlayerMoveDirection moveDirection = PlayerMoveDirection.Left;
    // true일 때 이동합니다.
    [Label("이동 모드"), Foldout("State"), SerializeField, ReadOnly] protected bool isMoving = false;
    [Label("벽타기"), Foldout("State"), SerializeField, ReadOnly] protected bool isClimbing = false;
    // Immune State
    [Label("현재 무적 시간"), Foldout("State")] public float immuneTime = 0f;
    [Label("피격시 무적 시간"), Foldout("State")] public float damageImmuneTime = 0.3f;
    [Label("현재 대시 무적 시간"), Foldout("State")] public float dashImmuneTime = 0f;
    [Label("대시시 무적 시간"), Foldout("State")] public float dashMaxImmuneTime = 0.1f;
    [Label("기본 넉백 지속 시간"), Foldout("State")] public float knockbackTime = 0.2f;
    [Label("기본 넉백 배율"), Foldout("State")] public float knockbackMultiplier = 2f;
    [Label("남은 정지 시간"), Foldout("State")] public float freezeTime = 0f;
    
    public bool IsImmune => isDamageImmune || dashImmuneTime > 0;
    private bool isDamageImmune = false;
    
    // 플레이어 스텟 관리 클래스입니다.
    [Label("캐릭터 스텟"), Foldout("State")] public PlayerStats stat = new();

    // Jump State
    [Label("점프 파워"), Foldout("Jump State")] public float jumpPower = 10;
    [Label("벽점프 파워"), Foldout("Jump State")] public float wallJumpPower = 10;
    [Label("벽점프 반동 제어불가 시간"), Foldout("Jump State")] public float wallJumpReactionTime = 0.4f; 
    [Label("최대 점프 시간"), Foldout("Jump State")] public float jumpTimeLimit = 0.2f;
    [Label("낙하시 중력가속도 보정값"), Foldout("Jump State")] public float gravityAcceleration = 10;
    [Label("낙하시 시작 중력값"), Foldout("Jump State")] public float gravityStart = -10;

    // 이 아래는 외부 접근용 연결 필드입니다.
    // 캐릭터가 공중인지 여부입니다.
    public bool IsFlight => !groundChecker.IsGround;
    public bool IsFalling => animator.GetBool(BOOL_FALLING);

    protected PlayerAnimationState AnimationState
    {
      get => animationState;
      set
      {
        animator.SetInteger(INT_STATE, (int) value);
        animationState = value;
      }
    }
    
    /// <summary>
    /// 플레이어 이동상태를 제어할 수 있습니다. <br/>
    /// false - 아무 행동하지 않음 / True - 뛰기
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public bool IsMoving
    {
      get => isMoving;
      set
      {
        if (!isMoving && value)
        {
          CancelBlock();
        }
        isMoving = value;
        AnimationState = value ? PlayerAnimationState.Run : PlayerAnimationState.Idle;
      }
    }
    
    /// <summary>
    /// 플레이어가 대시하고 있는지 여부입니다.
    /// </summary>
    public bool IsDashing => animator.GetCurrentAnimatorStateInfo(0).IsName("Dash") || dashCoroutine != null;

    private PlayerMoveDirection attackDirection;
    public PlayerMoveDirection MoveDirection
    {
      get => moveDirection;
      set => moveDirection = value;
    }
    
    #endregion
    
    #region Binding

    [Tooltip("캐릭터 바디"), Foldout("Bindings")] public Rigidbody2D body;
    [Tooltip("캐릭터 콜라이더"), Foldout("Bindings")] public CapsuleCollider2D bodyCollider;
    [Tooltip("캐릭터 애니메이터"), Foldout("Bindings"), SerializeField] protected Animator animator;
    [Tooltip("캐릭터 스프라이트"), Foldout("Bindings"), SerializeField] protected SpriteRenderer spriteRenderer;
    [Foldout("Bindings"), SerializeField] private PlayerGroundChecker groundChecker;
    [Foldout("Bindings"), SerializeField] private WorldGaugeBar dashGaugeBar, attackGaugeBar;

    #endregion

    #region Unity Event
    
#if UNITY_EDITOR

    private void Reset()
    {
      if (!body) body = GetComponent<Rigidbody2D>();
      if (!animator) animator = GetComponentInChildren<Animator>();
      if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    
#endif

    private void Awake()
    {
      InitDash();
      InitAttack();

      if (groundChecker)
      {
        groundChecker.onLanding.AddListener(() =>
        {
          animator.SetBool(BOOL_IS_FLIGHT, false);
        });
      }
    }

    private void Update()
    {
      if (freezeTime > 0)
      {
        freezeTime -= Time.unscaledDeltaTime;
      }
      else if(Time.timeScale == 0)
      {
        Time.timeScale = 1;
        freezeTime = 0;
      }
    }
    
    // 
    private void FixedUpdate()
    {
      AirStateAnimationHandle();            // 사전 구햔 3
      MoveInputHandle();                    // 사전 구현 4 - 벽타기는 여기
      TakeEnvironmentalForces();            // 사전 구현 5
      ImmunePropsHandle();                  // 사전 구현 1
      UpdateResources();
    }

    private void UpdateResources()
    {
      UpdateBlockResource();
      CharacterActionCooldownHandle();
    }

    private void AirStateAnimationHandle()
    {
      if (IsFlight)
      {
        animator.SetBool(BOOL_IS_FLIGHT, true);
      }
      
      var enterFallingAnim = body.linearVelocityY < -0.1f && !inWater &&
                             !IsDashing && !isAttacking;
      
      if (animator.GetBool(BOOL_FALLING) != enterFallingAnim)
      {
        animator.SetBool(BOOL_FALLING, enterFallingAnim);
        if (enterFallingAnim)
        {
          animator.SetTrigger(TRIGGER_FALL);
        }
      }
    }

    private void MoveInputHandle()
    {
      if (!isControlable) return;
      
      var dir = moveDirection == PlayerMoveDirection.Left ? Vector2.left : Vector2.right;
      transform.eulerAngles = new Vector3(0,
        (isAttacking ? attackDirection : MoveDirection) == PlayerMoveDirection.Left ? 180 : 0, 0);
      
      // isMoving이 true일떄 이동합니다.
      if(isMoving && !IsDashing &&
         (IsFlight || !IsAttackMotion))
      {
        // 최대이동속도 설정 및 이동 구현
        if (Math.Abs(body.linearVelocityX) < maxMoveSpeed)
        {
          body.AddForce(dir * moveSpeed, ForceMode2D.Impulse);
        }
      }

      if (isMoving && IsFlight && DetectWall(dir))
      {
        isClimbing = true;
        animator.SetBool(BOOL_CLIMB, true);
        
        if (body.linearVelocityY < 0) body.linearVelocityY = Mathf.Max(body.linearVelocityY, wallEnduringSpeed);
      }
      else
      {
        isClimbing = false;
        animator.SetBool(BOOL_CLIMB, false);
      }
    }

    private bool DetectWall(Vector2 dir)
    {
      Vector2 colliderCenterPos = (Vector2)transform.position + new Vector2(bodyCollider.offset.x * dir.x, bodyCollider.offset.y);
      Vector2 rayOrigin = colliderCenterPos + dir * (bodyCollider.size.x * 0.5f);
      
      if (Physics2D.Raycast(rayOrigin, dir, 0.03f, LayerMask.GetMask("Ground")))
      {
        return true;
      }
      return false;
    }

    private void CharacterActionCooldownHandle()
    {
      if (!IsFlight || isClimbing)
      {
        if (DashDelay > 0) DashDelay -= Time.deltaTime;
        else DashDelay = 0;
      }

      if (MeleeAttackDelay > 0) MeleeAttackDelay -= Time.deltaTime;
      else MeleeAttackDelay = 0;
    }

    private void TakeEnvironmentalForces()
    {
      // 이동시 마찰력 보정
      if(Math.Abs(body.linearVelocityX) > 1)
      {
        body.AddForce(-body.linearVelocity.normalized.Y(0) * moveResistanceForce, ForceMode2D.Impulse);
      }

      // 떨어질 떄 빨리 떨어지게
      if (body.linearVelocity.y < 0)
      {
        body.linearVelocity += Vector2.up * (Physics.gravity.y * (gravityAcceleration - 1) * Time.fixedDeltaTime);
      }
    }

    private void ImmunePropsHandle()
    {
      if (dashImmuneTime > 0) dashImmuneTime -= Time.deltaTime;
      else dashImmuneTime = 0;

      if (immuneTime > 0)
      {
        immuneTime -= Time.deltaTime;

        if (!isDamageImmune)
        {
          isDamageImmune = true;
          animator.SetBool(BOOL_IMMUNE, true);
        }
      }
      else
      {
        immuneTime = 0;
        if (isDamageImmune)
        {
          isDamageImmune = false;
          animator.SetBool(BOOL_IMMUNE, false);
        }
      }
    }

    #endregion
    
    #region Event

    /// <summary>
    /// 플레이어 사망시 호출됩니다.
    /// </summary>
    public UnityEvent OnDeath => stat.onDeath;
    
    /// <summary>
    /// 플레이어의 체력이 변경될 시 호출되며, 매개변수로 현재 체력을 넘겨줍니다.
    /// </summary>
    public UnityEvent<float> OnHpChange => stat.onHpChanged;
    
    #endregion Event
    
    #region Feature
    
    #region Jump Feature
    // 점프 관리용 코루틴
    private Coroutine jumpCoroutine = null;
    private Coroutine wallJumpControlLockCoroutine = null;
    
    /// <summary>
    /// Jump()를 호출하여 점프를 시작할 수 있습니다. <br/>
    /// 빠르게 CancelJump()를 호출하여 낮은 점프를 할 수 있습니다.
    /// </summary>
    public void Jump()
    {
      if (!isControlable) return;
      if ((inWater || (!IsFlight || isClimbing)) && !IsDashing && jumpCoroutine == null)
      {
        jumpCoroutine = StartCoroutine(JumpCoroutine());
      }
    }

    /// <summary>
    /// Jump()를 통한 점프가 끝나기전 호출해 점프 도중에 취소할 수 있습니다. 
    /// </summary>
    public void CancelJump()
    {
      if(jumpCoroutine != null)
      {
        StopCoroutine(jumpCoroutine);
        if(gravityStart != 0) body.linearVelocityY = gravityStart;
        jumpCoroutine = null;
      }
    }

    private IEnumerator JumpCoroutine()
    {
      animator.SetTrigger(TRIGGER_JUMP);

      if (isClimbing)
      {
        Vector2 kickReactionDir = moveDirection == PlayerMoveDirection.Left ? Vector2.right : Vector2.left;
        
        body.AddForce(kickReactionDir * (wallJumpPower * 1.4f), ForceMode2D.Impulse);   // 1.4는 약 루트2, 벽차기 약간 더 강하게
        transform.eulerAngles = new Vector3(0, kickReactionDir.x > 0 ? 0 : 180, 0);
        
        // 벽 반동에 의해 동작을 못하는 시간이라는 의미지만 반동 직후 사망시 코루틴에 의해 사망 컨트롤 락이 되어야 하는데 풀려버린다던가
        // 그런 식으로 다룰 가능성이 있어서 여기서 벽 반동 코루틴을 캐싱형태로 두었습니다 - 승화
        wallJumpControlLockCoroutine = StartCoroutine(WallJumpControlLock(wallJumpReactionTime));
      }

      var jumpTime = 0f;
      while (jumpTime < jumpTimeLimit)
      {
        body.linearVelocityY = jumpPower;
        jumpTime += Time.deltaTime;
        yield return new WaitForFixedUpdate();
      }
      
      if(gravityStart != 0) body.linearVelocityY = gravityStart;
      
      jumpCoroutine = null;
    }

    IEnumerator WallJumpControlLock(float time)
    {
      isControlable = false;
      
      yield return new WaitForSeconds(time);

      isControlable = true;
    }

    #endregion Jump Feature

    
    [Foldout("Effect"), SerializeField] private ParticleSystem damagedEffect;
    /// <summary>
    /// 플레이어에게 방어력을 반영한 체력 피해를 줍니다.<br/>
    /// 남은 체력 이상의 피해를 줄 시 자동으로 0까지만 내려가고, <br/>
    /// 0이 될 시 stats.onDeath 이벤트를 호출합니다.
    /// </summary>
    /// <param name="value">피해량입니다.</param>
    /// <param name="sender">피해량을 주는 주체입니다.</param>
    public void Damage(float value, MonoBehaviour sender)
    {
      if(IsImmune) return;

      if (IsBlocking && sender)
        Block(value, sender);
      else
      {
        stat.Damage(value);

        if (sender)
        {
          damagedEffect.transform.eulerAngles = Vector3.zero;

          if (damagedEffect.isPlaying) damagedEffect.Stop();
          damagedEffect.Play();
        }

        if (sender != null && immuneTime < damageImmuneTime) immuneTime = damageImmuneTime;
      }
    }

    /// <summary>
    /// 플레이어를 넉백시킵니다.
    /// </summary>
    /// <param name="value">넉백 세기입니다.</param>
    /// <param name="direction">넉백 방향입니다.</param>
    public void KnockBack(float value, Vector2 direction)
    {
      if(IsImmune) return;
      
      StartCoroutine(KnockBackCoroutine(value, direction));
    }
    
    /// <summary>
    /// 플레이어를 넉백시킵니다.
    /// </summary>
    /// <param name="value">넉백 세기입니다.</param>
    /// <param name="sender">넉백을 가하는 오브젝트입니다.</param>
    public void KnockBack(float value, GameObject sender) => KnockBack(value, sender.transform.position - transform.position);

    private IEnumerator KnockBackCoroutine(float value, Vector2 direction)
    {
      for (var time = 0f; time < knockbackTime; time += Time.deltaTime)
      {
        body.AddForce(direction * (value * knockbackMultiplier), ForceMode2D.Impulse);
        yield return new WaitForFixedUpdate();
      }
    }
    
    #endregion Feature
    
    #region Water
    [SerializeField, Foldout("State")] private List<WaterObject> waterObjects = new();
    
    private void WaterEnter(WaterObject obj)
    {
      if(waterObjects.Contains(obj)) return;
      
      waterObjects.Add(obj);
      if (waterObjects.Count > 0) inWater = true;
    }

    private void WaterExit(WaterObject obj)
    {
      if(!waterObjects.Contains(obj)) return;

      waterObjects.Remove(obj);
      if (waterObjects.Count == 0) inWater = false;
    }

    #endregion
    
    protected enum PlayerAnimationState
    {
      Idle = 0,
      Walk = 1,
      Run = 2,
    }
  }
  
  // 이동방향 설정용 열거형입니다.
  public enum PlayerMoveDirection
  {
    Left,
    Right,
  }
}