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
    [Tooltip("활성화된 메인 카메라"), SerializeField, ReadOnly] private new Camera camera;
    [Tooltip("시네머신 카메라"), SerializeField] protected CinemachineVirtualCamera vCam;
    [Tooltip("플레이어 캐릭터"), SerializeField] protected PlayerCharacter character;

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
        var characterPos = character.transform.position.Y(v => v);
        
        character.Attack((cursorPos - characterPos).normalized.Y(v => v), true);
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

    /// <summary>
    /// Input Action 용 메서드입니다. 호출하지 말아주세요!
    /// </summary>
    public void MeleeAttack(InputAction.CallbackContext context)
    {
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
    
    [SerializeField] private float interactRadius = 1f;
    [SerializeField] private LayerMask interactableMask;

    public void Interaction(InputAction.CallbackContext context)
    {
      if (context.performed) Interact();
    }

    private void Interact()
    {
      Collider2D[] hits = Physics2D.OverlapCircleAll(character.transform.position, interactRadius, interactableMask);
      IInteractable nearest = null;
      float nearestDistance = Mathf.Infinity;

      foreach (var hit in hits)
      {
        var interactable = hit.GetComponent<IInteractable>();
        if (interactable != null && interactable.IsInteractable)
        {
          float distance = Vector2.Distance(character.transform.position, hit.transform.position);
          if (distance < nearestDistance)
          {
            nearest = interactable;
            nearestDistance = distance;
          }
        }
      }

      if (nearest != null)
      {
        nearest.Interact();
      }
    }
    
    #endregion
  }
}