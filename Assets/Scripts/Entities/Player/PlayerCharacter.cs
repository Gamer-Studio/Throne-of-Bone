using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using ToB.Entities;
using ToB.Utils;
using ToB.Worlds;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace ToB.Player
{
  public class PlayerCharacter : MonoBehaviour, IDamageable
  {
    private static readonly int INT_STATE = Animator.StringToHash("State");
    private static readonly int BOOL_FALLING = Animator.StringToHash("Falling");
    private static readonly int TRIGGER_JUMP = Animator.StringToHash("Jump");
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
      
      return null;
    }
    
    #region State
    [Header( "State")]
    
    [Tooltip("애니메이션 상태"), SerializeField, GetSet(nameof(AnimationState))] protected PlayerAnimationState animationState = PlayerAnimationState.Idle;
    [Tooltip("이동속도")] public float moveSpeed = 2;
    [Tooltip("최대 이동 속도")] public float maxMoveSpeed = 12;
    [Tooltip("좌/우 마찰력 보정값")] public float moveResistanceForce = 1;
    [Tooltip("이동방향 (좌/우)")] public PlayerMoveDirection moveDirection = PlayerMoveDirection.Left;
    // true일 때 이동합니다.
    [Tooltip("이동 모드"), SerializeField, ReadOnly] protected bool isMoving = false;
    [Tooltip("체공 여부"), SerializeField, ReadOnly] private bool isFlight = false;

    [Header("Jump State")]
    [Tooltip("점프 파워")] public float jumpPower = 10;
    [Tooltip("최대 점프 시간")] public float jumpTimeLimit = 0.2f;
    [Tooltip("낙하시 중력가속도 보정값")] public float gravityAcceleration = 10;
    
    [Header("Dash State")]
    [Tooltip("대시 보정값")] public float dashMultiplier = 12;
    [Tooltip("대시 지속시간")] public float dashTimeLimit = 0.2f;
    [FormerlySerializedAs("isWater")] [Tooltip("물속인지")] public bool inWater = false;

    // 플레이어 스텟 관리 클래스입니다.
    public PlayerStats stat = new();

    // 이 아래는 외부 접근용 연결 필드입니다.
    public bool IsFlight => isFlight;

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
        isMoving = value;
        AnimationState = value ? PlayerAnimationState.Run : PlayerAnimationState.Idle;
      }
    }

    /// <summary>
    /// 공격 모션이 재생되고 있는지 여부를 반환합니다.
    /// </summary>
    public bool IsAttacking => animator.GetCurrentAnimatorStateInfo(0).IsName("Slash0") ||
                               animator.GetCurrentAnimatorStateInfo(0).IsName("Slash1") ||
                               animator.GetCurrentAnimatorStateInfo(0).IsName("Slash2");
    
    public bool IsDashing => animator.GetCurrentAnimatorStateInfo(0).IsName("Dash") || dashCoroutine != null;
    
    #endregion
    
    #region Binding
    [Header("Bindings")]
    
    [Tooltip("캐릭터 바디")] public Rigidbody2D body;
    [Tooltip("캐릭터 애니메이터"), SerializeField] protected Animator animator;
    [Tooltip("캐릭터 스프라이트"), SerializeField] protected SpriteRenderer spriteRenderer;
    
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

    private void FixedUpdate()
    {
      dashDelay -= Time.deltaTime;
      isFlight = Math.Abs(body.linearVelocityY) > 0.1f;

      var inDash = animator.GetCurrentAnimatorStateInfo(0).IsName("Dash");
      var enterFallingAnim = body.linearVelocityY < -0.1f && !inWater &&
                          !inDash && !IsAttacking;
      
      if (animator.GetBool(BOOL_FALLING) != enterFallingAnim)
      {
        animator.SetBool(BOOL_FALLING, enterFallingAnim);
      }
      
      // isMoving이 true일떄 이동합니다.
      if(isMoving && !inDash)
      {
        transform.eulerAngles = new Vector3(0, moveDirection == PlayerMoveDirection.Left ? 180 : 0, 0);
        
        // 최대이동속도 설정 및 이동 구현
        if (Math.Abs(body.linearVelocityX) < maxMoveSpeed)
          body.AddForce(moveDirection == PlayerMoveDirection.Left ? Vector2.left * moveSpeed : Vector2.right * moveSpeed,
            ForceMode2D.Impulse);
      }
      
      // 이동시 마찰력 보정
      if(Math.Abs(body.linearVelocityX) > 1)
      {
        body.AddForce(-body.linearVelocity.normalized.Y(0) * moveResistanceForce, ForceMode2D.Impulse);
      }
      
      // 떨어질 떄 빨리 떨어지게
      if (body.linearVelocity.y < 0)
      {
        body.linearVelocity += Vector2.up * (Physics.gravity.y * (gravityAcceleration - 1) * Time.deltaTime);
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
    
    /// <summary>
    /// Jump()를 호출하여 점프를 시작할 수 있습니다. <br/>
    /// 빠르게 CancelJump()를 호출하여 낮은 점프를 할 수 있습니다.
    /// </summary>
    [Button]
    public void Jump()
    {
      if ((inWater || !isFlight) && !IsDashing && jumpCoroutine == null)
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
        jumpCoroutine = null;
      }
    }

    private IEnumerator JumpCoroutine()
    {
      animator.SetTrigger(TRIGGER_JUMP);

      var jumpTime = 0f;
      while (jumpTime < jumpTimeLimit)
      {
        body.AddRelativeForce(Vector2.up * (jumpPower * (Time.deltaTime / jumpTimeLimit)), ForceMode2D.Impulse);
        jumpTime += Time.deltaTime;
        yield return new WaitForFixedUpdate();
      }
      
      jumpCoroutine = null;
    }
    
    #endregion Jump Feature
    
    #region Dash Feature
    
    public float dashDelay = 0;
    public float dashCoolTime = 0.5f;

    private Coroutine dashCoroutine = null;
    /// <summary>
    /// 현재 플레이어의 방향으로 돌진합니다.
    /// </summary>
    public void Dash()
    {
      if(dashDelay <= 0 && !IsDashing && body.gravityScale != 0)
      {
        dashDelay = 20;
        CancelJump();
        StartCoroutine(DashCoroutine());
      }
    }

    private IEnumerator DashCoroutine()
    {
      animator.SetTrigger(TRIGGER_DASH);
      var beforeGravityScale = body.gravityScale;
      body.gravityScale = 0;
      body.linearVelocity = Vector2.zero;
      
      animator.SetInteger(INT_DASH_STATE, 0);
      var dashTime = 0f;
      while (dashTime < dashTimeLimit)
      {
        body.AddForce((moveDirection == PlayerMoveDirection.Left ? Vector2.left : Vector2.right) *
                              (moveSpeed * dashMultiplier * Time.deltaTime), ForceMode2D.Impulse);
        dashTime += Time.deltaTime;
        yield return new WaitForFixedUpdate();
      }

      animator.SetInteger(INT_DASH_STATE, 1);
      body.gravityScale = beforeGravityScale;
      body.linearVelocityX = 0;
      body.linearVelocityY = -0.1f;
      isFlight = true;
      dashCoroutine = null;

      dashDelay = dashCoolTime;
    }
    
    #endregion Dash Feature
    
    #region Attack Feature
    
    /// <summary>
    /// direction 방향으로 공격합니다.
    /// isMelee를 false로 하여 원거리 공격을 할 수 있습니다.
    /// </summary>
    /// <param name="direction">공격 방향입니다.</param>
    /// <param name="isMelee">근거리/원거리 공격 방향입니다.</param>
    public void Attack(Vector2 direction, bool isMelee = true)
    {
      if (IsAttacking) return;
      // 애니메이션 구현
      animator.SetTrigger(TRIGGER_ATTACK);
      var prevMotion = animator.GetInteger(INT_ATTACK_MOTION);
      prevMotion = prevMotion == 2 ? 0 : prevMotion + 1;
      animator.SetInteger(INT_ATTACK_MOTION, prevMotion);
      
      var dir = direction.x > 0 ? PlayerMoveDirection.Right : PlayerMoveDirection.Left;
      
      if (direction.x > 0)
      {
          
      }
      else
      {
          
      }

      // 원거리 공격 구현
      if (!isMelee)
      {
        
      }
    }

    /// <summary>
    /// 이벤트 트리거에요. 호출하지 말아주세요!
    /// </summary>
    public void AttackEnd()
    {
      
    }
    
    #endregion Attack Feature

    /// <summary>
    /// 플레이어에게 방어력을 반영한 체력 피해를 줍니다.<br/>
    /// 남은 체력 이상의 피해를 줄 시 자동으로 0까지만 내려가고, <br/>
    /// 0이 될 시 stats.onDeath 이벤트를 호출합니다.
    /// </summary>
    /// <param name="value">피해량입니다.</param>
    /// <returns>플레이어의 남은 체력입니다.</returns>
    public void Damage(float value, MonoBehaviour sender) => stat.Damage(value);
    
    #endregion Feature
    
    #region Water
    [SerializeField] private List<WaterObject> waterObjects = new();
    
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