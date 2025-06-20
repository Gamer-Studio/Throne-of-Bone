using System;
using NaughtyAttributes;
using ToB.Utils;
using UnityEngine;

namespace ToB.Player
{
  public class PlayerCharacter : MonoBehaviour
  {
    private static readonly int state = Animator.StringToHash("State");
    private static readonly int jump = Animator.StringToHash("Jump");

    #region State
    
    [SerializeField, GetSet(nameof(AnimationState))] protected PlayerAnimationState animationState = PlayerAnimationState.Idle;
    public float jumpForce = 10;
    public float gravityAcceleration = 10;
    public float moveSpeed = 2;
    public float maxMoveSpeed = 4;
    public float moveResistanceForce = 1;
    public PlayerMoveDirection moveDirection = PlayerMoveDirection.Left;
    [SerializeField, ReadOnly] protected PlayerMoveMode moveMode = PlayerMoveMode.Idle;

    #endregion

    public PlayerMoveMode MoveMode
    {
      get => moveMode;
      set
      {
        moveMode = value;
        switch (value)
        {
          case PlayerMoveMode.Idle: AnimationState = PlayerAnimationState.Idle; break;
          case PlayerMoveMode.Walk: AnimationState = PlayerAnimationState.Walk; break;
          case PlayerMoveMode.Run: AnimationState = PlayerAnimationState.Run; break;
          default: throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }
      }
    }
    
    protected PlayerAnimationState AnimationState
    {
      get => animationState;
      set
      {
        animator.SetInteger(state, (int) value);
        animationState = value;
      }
    }

    #region Binding
    
    public Rigidbody2D body;
    [SerializeField] protected Animator animator;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    
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
      
      if(moveMode != PlayerMoveMode.Idle)
      {
        spriteRenderer.flipX = moveDirection == PlayerMoveDirection.Left;
        
        // 최대이동속도 설정 및 이동 구현
        if (Math.Abs(body.linearVelocity.x) < (moveMode == PlayerMoveMode.Run ? maxMoveSpeed * 2 : maxMoveSpeed))
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
    
    #region Feature
    
    public void Jump()
    {
      if (Math.Abs(body.linearVelocity.y) > 0.1f) return;
      
      animator.SetTrigger(jump);
      body.AddRelativeForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    
    #endregion
    
    protected enum PlayerAnimationState
    {
      Idle = 0,
      Walk = 1,
      Run = 2,
    }
  }
  
  public enum PlayerMoveDirection
  {
    Left,
    Right,
  }

  public enum PlayerMoveMode
  {
    Idle,
    Walk,
    Run
  }
}