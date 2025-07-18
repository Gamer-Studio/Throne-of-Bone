using System.Collections.Generic;
using ToB.Core;
using ToB.Scenes.Stage;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ToB.UI
{
    public class MainBookUI:UIPanelBase
    {
        [SerializeField] public GameObject[] panelObjects;
        private GameObject currentPanel;
        [SerializeField] public Button[] buttons;
        private List<Image> panelImages = new List<Image>();
        private void Awake()
        {
           InitButtons();
           InitPanels();
        }

        private void InitButtons()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                int index = i;
                buttons[i].onClick.AddListener(() => ShowPanel(index));
                // 이벤트 리스너 각각 등록
            }
        }
        private void InitPanels()
        {
            for (int i = 0; i < panelObjects.Length; i++)
            {
                panelImages.Add(panelObjects[i].GetComponent<Image>());
            }
            
            if (currentPanel == null)
            {
                panelObjects[0].SetActive(true);
            }
            else
            {
                currentPanel.SetActive(true);
            }
        }
        
        private void ShowPanel(int indexToShow)
        {
            StageManager.Instance?.ChangeGameState(GameState.UI);
            for (int i = 0; i < panelObjects.Length; i++)
            {
                if (i == indexToShow)
                {
                    panelObjects[i].SetActive(true);
                    panelImages[i].raycastTarget = true;
                    currentPanel = panelObjects[i];
                }
                else
                {
                    panelObjects[i].SetActive(false);
                    panelImages[i].raycastTarget = false;
                }
                // true인 것만 SetActive, 나머진 false
            }
        }

        public void BackToMainMenuScene()
        {
            this.gameObject.SetActive(false);
            SceneManager.LoadScene(Defines.MainMenuScene);
        }

        #region InputAction
        public void SkillUIToggle(InputAction.CallbackContext context)
        {
            //분기 1 : 메인북이 꺼져 있으면 켠다
            // 분기 2-1 : 메인북이 켜져 있을 경우, 현재 패널을 또 부르면 끈다
            // 분기 2-2 : 메인북이 켜져 있을 경우, 다른 패널을 부르면 패널을 바꾼다
            if (context.performed)
            {
                if (gameObject.activeSelf && currentPanel == panelObjects[0])
                {
                    CloseBook();
                }
                else if (gameObject.activeSelf && currentPanel != panelObjects[0])
                {
                    ShowPanel(0);
                }
                else if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                    UIManager.Instance.panelStack.Push(this);
                    UIManager.Instance.wideMapUI.gameObject.SetActive(false);
                    ShowPanel(0);
                }
            }
        }

        public void SoulUIToggle(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (gameObject.activeSelf && currentPanel == panelObjects[1])
                {
                    CloseBook();
                }
                else if (gameObject.activeSelf && currentPanel != panelObjects[1])
                {
                    ShowPanel(1);
                }
                else if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                    UIManager.Instance.wideMapUI.gameObject.SetActive(false);
                    UIManager.Instance.panelStack.Push(this);
                    ShowPanel(1);
                }
            }
        }
        
        public void CollectionUIToggle(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (gameObject.activeSelf && currentPanel == panelObjects[2])
                {
                    CloseBook();
                }
                else if (gameObject.activeSelf && currentPanel != panelObjects[2])
                {
                    ShowPanel(2);
                }
                else if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                    UIManager.Instance.wideMapUI.gameObject.SetActive(false);
                    UIManager.Instance.panelStack.Push(this);
                    ShowPanel(2);
                }
            }
        }

        public void StatisticsUIToggle(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (gameObject.activeSelf && currentPanel == panelObjects[3])
                {
                    CloseBook();
                }
                else if (gameObject.activeSelf && currentPanel != panelObjects[3])
                {
                    ShowPanel(3);
                }
                else if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                    UIManager.Instance.wideMapUI.gameObject.SetActive(false);
                    UIManager.Instance.panelStack.Push(this);
                    ShowPanel(3);
                }
            }
        }

        public void SettingUIToggle(InputAction.CallbackContext context)
        {
            if (context.performed && !UIManager.Instance.isThereActiveUI)
            {
                if (gameObject.activeSelf && currentPanel == panelObjects[4])
                {
                    CloseBook();
                }
                else if (gameObject.activeSelf && currentPanel != panelObjects[4])
                {
                    ShowPanel(4);
                }
                else if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                    UIManager.Instance.wideMapUI.gameObject.SetActive(false);
                    UIManager.Instance.panelStack.Push(this);
                    ShowPanel(4);
                }
            }
        }
#endregion


        public override void Process(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }
        
        public override void Cancel(InputAction.CallbackContext context)
        {
            CloseBook();
        }

        private void CloseBook()
        {
            gameObject.SetActive(false);
            UIManager.Instance.panelStack.Pop();
            if(StageManager.Instance && UIManager.Instance.panelStack.Count == 0) 
                StageManager.Instance.ChangeGameState(GameState.Play);
        }
    }
}