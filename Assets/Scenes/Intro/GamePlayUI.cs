using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;   
namespace ToB.Scenes.Intro
{
    public class GamePlayUI:MonoBehaviour
    {
        [SerializeField] public GameObject playerInfoPanel;
        [SerializeField] public GameObject miniMapPanel;
        
        private void Awake()
        {
            UIManager.Instance.Init(this);
            playerInfoPanel.SetActive(true);
            miniMapPanel.SetActive(true);
        }
        
        public void ClearActiveUI(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Debug.Log("ClearActiveUI 실행됨");
                if (UIManager.Instance.IsThereOverlayUI())
                {
                    Debug.Log("활성화된 UI 존재함. UI 닫음.");
                    UIManager.Instance.wideMapUI.gameObject.SetActive(false);
                    UIManager.Instance.mainBookUI.gameObject.SetActive(false);
                }
            }
        }
    }
}