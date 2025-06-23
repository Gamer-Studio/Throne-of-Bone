using System;
using Cinemachine;
using NaughtyAttributes;
using ToB.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ToB.Player
{
  public class PlayerController : MonoBehaviour
  {
    [SerializeField, Tooltip("시네머신 카메라")] protected CinemachineVirtualCamera vCam;
    [SerializeField, Tooltip("플레이어 캐릭터")] protected PlayerCharacter character;

    #region Unity Event
    
#if UNITY_EDITOR

    private void Reset()
    {
      if (!vCam) vCam = FindAnyObjectByType<CinemachineVirtualCamera>();
      if (!character) character = FindAnyObjectByType<PlayerCharacter>();
    }
    
#endif

    #endregion
    
    #region Input Action

    public void Move(InputAction.CallbackContext context)
    {
      var input = context.ReadValue<Vector2>().x;
      if (Math.Abs(input) > 0.1f)
      {
        character.moveDirection = input > 0 ? PlayerMoveDirection.Right : PlayerMoveDirection.Left;
        character.MoveMode = PlayerMoveMode.Run;
      }
      else
      {
        character.MoveMode = PlayerMoveMode.Idle;
      }
    }

    public void Jump(InputAction.CallbackContext context)
    {
      if (context.performed) character.Jump();
      else if (context.canceled) character.CancelJump();
    }

    public void Dash(InputAction.CallbackContext context)
    {
      if (context.performed) character.Dash();
    }
    
    #endregion
  }
}