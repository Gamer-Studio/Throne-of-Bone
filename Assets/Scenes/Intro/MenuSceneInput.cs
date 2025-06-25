using UnityEngine;
using UnityEngine.InputSystem;

namespace ToB.Scenes.Intro
{
    public class MenuSceneInput : MonoBehaviour
    {
        public void CancelUI(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Debug.Log("UIManager - ESC 인식");
                CloseActiveUI();
            }
        }

        private void CloseActiveUI()
        {
            Debug.Log("뒤로 가기 작업 : UI 구조 개선 후 진행");
        }
    }
}
