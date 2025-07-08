using System;
using Cinemachine;
using NaughtyAttributes;
using ToB.Entities.Obstacle;
using ToB.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ToB.Player
{
  public class PlayerController : MonoBehaviour
  {
    [Label("활성화된 메인 카메라"), SerializeField, ReadOnly] private new Camera camera;
    [Label("시네머신 카메라"), SerializeField] protected CinemachineVirtualCamera vCam;
    [Label("플레이어 캐릭터"), SerializeField] protected PlayerCharacter character;

    private bool isMeleeAttacking = false;
    private bool isRangedAttacking = false;
    
    #region Unity Event
    
#if UNITY_EDITOR

    private void Reset()
    {
      if (!vCam) vCam = FindAnyObjectByType<CinemachineVirtualCamera>();
      if (!character) character = FindAnyObjectByType<PlayerCharacter>();
    }
    
#endif

    private void Awake()
    {
      camera = Camera.main;
    }

    private void FixedUpdate()
    {
      if (isMeleeAttacking)
      {
        var cursorPos = camera.ScreenToWorldPoint(Input.mousePosition).Z(0);
        var characterPos = character.transform.position;
        
        character.Attack((cursorPos - characterPos).normalized, true);
      }
      else if (isRangedAttacking)
      {
        var cursorPos = camera.ScreenToWorldPoint(Input.mousePosition).Z(0);
        var characterPos = character.transform.position.Y(v => v);
        var angle = (cursorPos - characterPos).normalized.Y(v => v);
        
        character.Attack(angle, false);
      }
    }

    #endregion
    
    #region Input Action

    /// <summary>
    /// Input Action 용 메서드입니다. 호출하지 말아주세요!
    /// </summary>
    public void Move(InputAction.CallbackContext context)
    {
      if(!character) return;
      
      var input = context.ReadValue<Vector2>().x;
      if (Math.Abs(input) > 0.1f)
      {
        character.MoveDirection = input > 0 ? PlayerMoveDirection.Right : PlayerMoveDirection.Left;
        character.IsMoving = true;
      }
      else
      {
        character.IsMoving = false;
      }
    }

    /// <summary>
    /// Input Action 용 메서드입니다. 호출하지 말아주세요!
    /// </summary>
    public void Jump(InputAction.CallbackContext context)
    {
      if (context.performed) character.Jump();
      else if (context.canceled) character.CancelJump();
    }

    /// <summary>
    /// Input Action 용 메서드입니다. 호출하지 말아주세요!
    /// </summary>
    public void Dash(InputAction.CallbackContext context)
    {
      if (context.performed) character.Dash();
    }

    private static readonly Vector2[] directions = { Vector2.right, Vector2.left, Vector2.down };
    
    /// <summary>
    /// Input Action 용 메서드입니다. 호출하지 말아주세요!
    /// </summary>
    public void MeleeAttack(InputAction.CallbackContext context)
    {
      if (!character.IsFlight || !context.performed)
      {
        isMeleeAttacking = context.performed;
        return;
      }
      
      var cursorPos = camera.ScreenToWorldPoint(Input.mousePosition).Z(0);
      var characterPos = character.transform.position.Y(v => v);
      var direction = (cursorPos - characterPos).normalized;
      
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
      
      if (closest == Vector2.down)
      {
        character.Attack(direction, true, true);
      }
      else
        isMeleeAttacking = context.performed;
    }

    /// <summary>
    /// Input Action 용 메서드입니다. 호출하지 말아주세요!
    /// </summary>
    public void RangedAttack(InputAction.CallbackContext context)
    {
      isRangedAttacking = context.performed;
    }
    
    #endregion
    
    #region Interaction
    
    [Label("상호작용 범위 반지름"), SerializeField] private float interactRadius = 1f;
    [Label("상호작용 레이어 마스크"), SerializeField] private LayerMask interactableMask;

    /// <summary>
    /// Input Action 용 메서드입니다. 호출하지 말아주세요!
    /// </summary>
    public void Interaction(InputAction.CallbackContext context)
    {
      if (context.performed)
      {
        Interact();
      }
    }

    /// <summary>
    /// interactable에 상호작용합니다.
    /// </summary>
    /// <param name="interactable">상호작용할 대상입니다.</param>
    public void Interact()
    { 
      var hits = Physics2D.OverlapCircleAll(character.transform.position, interactRadius, interactableMask);
      
      var nearestDistance = Mathf.Infinity;
      IInteractable nearest = null;
      Vector3? nearestPos = null;
      
      foreach (var hit in hits)
      {
        if (hit.TryGetComponent<IInteractable>(out var interactable) && 
            interactable is not { IsInteractable: true }) continue;

        var distance = Vector2.Distance(character.transform.position, hit.transform.position);

        if (distance >= nearestDistance) continue;

        nearest = interactable;
        nearestDistance = distance;
        nearestPos = hit.transform.position;
      }
      
      // 디버그: 오버랩 반경 그리기 (씬 뷰에서만 보임)
      DebugDrawCircle(character.transform.position, interactRadius, Color.cyan, 0.5f);

      if (nearest != null && nearestPos.HasValue)
      {
        // 디버그: 가장 가까운 대상까지 선
        Debug.DrawLine(character.transform.position, nearestPos.Value, Color.green, 0.5f);
      }
      
      Debug.Log(nearest);
      nearest?.Interact();
    }
    void DebugDrawCircle(Vector3 center, float radius, Color color, float duration = 0.1f, int segments = 32)
    {
      float angleStep = 360f / segments;
      Vector3 prevPoint = center + Quaternion.Euler(0, 0, 0) * Vector3.right * radius;

      for (int i = 1; i <= segments; i++)
      {
        float angle = angleStep * i;
        Vector3 nextPoint = center + Quaternion.Euler(0, 0, angle) * Vector3.right * radius;
        Debug.DrawLine(prevPoint, nextPoint, color, duration);
        prevPoint = nextPoint;
      }
    }
    #endregion
  }
}