using UnityEngine;
using UnityEngine.InputSystem;

namespace ToB.UI.WideMap
{
    public class WideMapUI:UIPanelBase
    {
        [SerializeField] private GameObject wideMapPanel;
        [SerializeField] private GameObject TPPanel;
        
        public void WideMapToggle(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                // 분기 1. 전체맵이 꺼져 있는 경우 : 다 끄고 전체맵 켜기
                if (!gameObject.activeSelf)
                {
                    UIManager.Instance.mainBookUI.gameObject.SetActive(false);
                    // 윗 부분은 추후 리팩토링 때 UIManager에서 매서드로 OverLayUI 끄는 메서드로 묶기
                    gameObject.SetActive(true);
                    wideMapPanel.SetActive(true);
                }
                // 분기 2. 전체맵이 켜져 있는 경우 : 끄기
                else if (gameObject.activeSelf)
                {
                    gameObject.SetActive(false);
                    wideMapPanel.SetActive(false);
                }
            }
        }

        public void TPPanelToggle()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                TPPanel.SetActive(true);
            }
            else if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                TPPanel.SetActive(false);
            }
        }
        


        public override void Process(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public override void Cancel(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}