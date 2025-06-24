using System;
using System.Collections;
using System.Linq;
using NaughtyAttributes;
using ToB.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace ToB.Player
{
  public class PlayerCharacter : MonoBehaviour
  {
    private static readonly int INT_STATE = Animator.StringToHash("State");
    private static readonly int TRIGGER_FALL = Animator.StringToHash("Fall");
    private static readonly int BOOL_IS_FLIGHT = Animator.StringToHash("IsFlight");
    private static readonly int TRIGGER_JUMP = Animator.StringToHash("Jump");
    private static readonly int TRIGGER_DASH = Animator.StringToHash("Dash");
    private static readonly int INT_DASH_STATE = Animator.StringToHash("DashState");

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
    /// Idle - 아무 행동하지 않음 / Run - 뛰기
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
      isFlight = Math.Abs(body.linearVelocity.y) > 0.2f;
      animator.SetBool(BOOL_IS_FLIGHT, isFlight);

      if (!isFlight && jumpCoroutine == null)
        isJumping = false;
      
      if(isFlight && !isJumping)
        animator.SetTrigger(TRIGGER_FALL);
      
      // isMoving이 true일떄 이동합니다.
      if(isMoving)
      {
        transform.eulerAngles = new Vector3(0, moveDirection == PlayerMoveDirection.Left ? 180 : 0, 0);
        
        // 최대이동속도 설정 및 이동 구현
        if (Math.Abs(body.linearVelocity.x) < maxMoveSpeed)
          body.AddForce(moveDirection == PlayerMoveDirection.Left ? Vector2.left * moveSpeed : Vector2.right * moveSpeed,
            ForceMode2D.Impulse);
      }
      
      // 이동시 마찰력 보정
      if(Math.Abs(body.linearVelocity.x) > 1)
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

    public UnityEvent OnDeath => stat.onDeath;
    public UnityEvent<int> OnHpChange => stat.onHpChanged;
    
    #endregion
    
    #region Feature
    
    #region Jump Feature
    // 점프 관리용 코루틴
    private Coroutine jumpCoroutine = null;
    [SerializeField] private bool isJumping = false;
    
    /// <summary>
    /// Jump()를 호출하여 점프를 시작할 수 있습니다.
    /// 빠르게 CancelJump()를 호출하여 낮은 점프를 할 수 있습니다.
    /// </summary>
    [Button]
    public void Jump()
    {
      if (isFlight) return;
      
      animator.SetTrigger(TRIGGER_JUMP);
      jumpCoroutine ??= StartCoroutine(JumpCoroutine());
      isJumping = true;
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

    private Coroutine dashCoroutine = null;
    /// <summary>
    /// 현재 플레이어의 방향으로 돌진합니다.
    /// </summary>
    public void Dash()
    {
      if(dashCoroutine == null) StartCoroutine(DashCoroutine());

      animator.SetTrigger(TRIGGER_DASH);
    }

    private IEnumerator DashCoroutine()
    {
      animator.SetInteger(INT_DASH_STATE, 0);
      var dashTime = 0f;
      while (dashTime < dashTimeLimit)
      {
        if(dashTime > dashTimeLimit / 2) animator.SetInteger(INT_DASH_STATE, 1);
        
        body.AddRelativeForce((moveDirection == PlayerMoveDirection.Left ? Vector2.left : Vector2.right) *
                              (moveSpeed * dashMultiplier * (Time.deltaTime / dashTimeLimit)), ForceMode2D.Impulse);
        dashTime += Time.deltaTime;
        yield return new WaitForFixedUpdate();
      }
      animator.SetInteger(INT_DASH_STATE, 2);
      
      dashCoroutine = null;
    }
    
    #endregion Dash Feature
    
    #region Attack Feature
    

    private static readonly Vector2[] DIRECTIONS = {Vector2.left, Vector2.right, Vector2.up, Vector2.down};
    
    /// <summary>
    /// direction 방향으로 공격합니다.
    /// isMelee를 false로 하여 원거리 공격을 할 수 있습니다.
    /// </summary>
    /// <param name="direction">공격 방향입니다.</param>
    /// <param name="isMelee">근거리/원거리 공격 방향입니다.</param>
    public void Attack(Vector2 direction, bool isMelee = true)
    {
      var targetDir = direction;
      if (isMelee)
      {
        targetDir = (from dir in DIRECTIONS orderby Vector2.Distance(dir, direction) select dir).First();

        
      }
      else
      {
        
      }
    }
    
    #endregion Attack Feature

    public int Damage(int value)
    {
      stat.Hp -= value;
      return stat.Hp;
    }
    
    #endregion Feature
    
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