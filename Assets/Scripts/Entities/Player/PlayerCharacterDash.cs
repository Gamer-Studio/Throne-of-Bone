using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace ToB.Player
{
  public partial class PlayerCharacter
  {
    #region State
    
    [Label("대시 속도"), Foldout("Dash State")] public float dashSpeed = 500;
    [Label("대시 지속시간"), Foldout("Dash State")] public float dashTimeLimit = 0.2f;
    [Label("대시 쿨타임 상태"), Foldout("Dash State")] private float dashDelay = 0;
    [Label("대시 쿨타임"), Foldout("Dash State")] public float dashCoolTime = 0.5f;
    [Label("물속인지"), Foldout("Dash State")] public bool inWater = false;
    [Label("대시 무적 시간 시점"), Foldout("Dash State")] public float dashImmuneStartTime = 0f; 

    public float DashDelay
    {
      get => dashDelay;
      set => dashGaugeBar.Value = dashDelay = value;
    }
    
    #endregion
    
    private float beforeGravityScale = 0;

    private Coroutine dashCoroutine = null;

    private void InitDash()
    {
      dashGaugeBar.max = dashCoolTime;
    }
    
    /// <summary>
    /// 현재 플레이어의 방향으로 돌진합니다.
    /// </summary>
    public void Dash()
    {
      if(dashDelay <= 0 && !IsDashing && body.gravityScale != 0)
      {
        dashDelay = 20;
        CancelJump();
        dashCoroutine = StartCoroutine(DashCoroutine());
      }
    }

    /// <summary>
    /// 대시하는 중이라면 대시 모션을 캔슬합니다.
    /// </summary>
    public void CancelDash()
    {
      if(dashCoroutine != null)
      {
        StopCoroutine(dashCoroutine);
        dashCoroutine = null;
        
        animator.SetInteger(INT_DASH_STATE, 1);
        body.gravityScale = beforeGravityScale;
        body.linearVelocityX = 0;
        body.linearVelocityY = -0.1f;
        dashCoroutine = null;

        DashDelay = dashCoolTime;
      }
    }

    private IEnumerator DashCoroutine()
    {
      var immuned = false;
      
      animator.SetTrigger(TRIGGER_DASH);
      beforeGravityScale = body.gravityScale;
      body.gravityScale = 0;
      body.linearVelocity = Vector2.zero;
      
      animator.SetInteger(INT_DASH_STATE, 0);
      var dashTime = 0f;
      while (dashTime < dashTimeLimit)
      {
        body.linearVelocityX = (moveDirection == PlayerMoveDirection.Left ? -1 : 1) * dashSpeed;
        
        dashTime += Time.deltaTime;
        if (!immuned && dashTime > dashImmuneStartTime)
        {
          immuned = true;
          dashImmuneTime = dashMaxImmuneTime;
        }
        
        yield return new WaitForFixedUpdate();
      }

      animator.SetInteger(INT_DASH_STATE, 1);
      body.gravityScale = beforeGravityScale;
      body.linearVelocity = Vector2.zero;
      dashCoroutine = null;

      DashDelay = dashCoolTime;
    }
  }
}