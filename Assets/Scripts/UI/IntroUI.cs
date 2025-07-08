using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace ToB.UI
{
    public class IntroUI : MonoBehaviour
    {
        [SerializeField] public GameObject MainPanel;
        [SerializeField] public GameObject SaveSlotPanel;
        [SerializeField] public GameObject SettingPanel;
        
        [Header("Setting Panel")]
        
        [Header("Save Slot Panel")]
        [SerializeField] public GameObject ConformPanel;
        [SerializeField] public Button[] saveSlotButtons;
        private string saveFileNameCashing = "";
        
        private void Awake()
        {
            UIManager.Instance.Init(this);
        }

        private void SaveSlotsInit()
        {
            for (int i = 0; i < saveSlotButtons.Length; i++)
            {
                int index = i;
                saveSlotButtons[i].onClick.AddListener(() => SaveSlotSelected(index));
                saveSlotButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = $"Save Slot {i + 1}";
            }
        }

        #region UIStack
        
        private Stack<GameObject> uiStack = new Stack<GameObject>();

        // 이거는 사용할 일이 없을 거 같긴 한데...
        public void OpenMainPanel()
        {
            MainPanel.SetActive(true);
        }
        
        public void OpenPanel(GameObject panel)
        {
            if (panel == null) return;

            if (uiStack.Count == 0)
            {
                MainPanel.SetActive(false);
            }

            // 이전 패널 비활성화
            if (uiStack.Count > 0)
            {
                GameObject topPanel = uiStack.Peek();
                topPanel.SetActive(false);
            }

            // 새 패널 활성화
            panel.SetActive(true);
            uiStack.Push(panel);
        }
        public void CloseCurrentPanel(InputAction.CallbackContext context)
        {
            if (!context.performed) return;


            if (uiStack.Count > 0)
                ClosePanel();
            else
                Debug.Log("더 이상 닫을 패널이 없습니다.");
        }
        
        

        public void ClosePanel()
        {
            // 스택에 패널이 없으면 리턴
            if (uiStack.Count == 0) return;
            
            // 제일 상위 패널 끄기
            GameObject topPanel = uiStack.Pop();
            topPanel.SetActive(false);
            
            // 아래 패널 켜기
            if (uiStack.Count > 0)
            {
                GameObject prevPanel = uiStack.Peek();
                prevPanel.SetActive(true);
            }
            // 더이상 아래 패널이 없으면 메인 패널 켜기
            else if (uiStack.Count == 0)
            {
                MainPanel.SetActive(true);
            }
        }

        public void CloseAllPanels()
        {
            // 스택에 있는 모든 패널 제거
            while (uiStack.Count > 0)
            {
                var panel = uiStack.Pop();
                panel.SetActive(false);
            }
            OpenMainPanel();
        }
        
        #endregion
        
        #region ButtonAction

        public void BackToMainMenuScene()
        {
            CloseAllPanels();
            SceneManager.LoadScene("MainMenu");
            Debug.Log("메인 메뉴");
        }
        
        public void StartGame()
        {
            CloseAllPanels();
            SceneManager.LoadScene("Stage_Manager");
            Debug.Log("게임 시작");
        }
        
        public void LoadGame()
        {
            CloseAllPanels();
            SceneManager.LoadScene("Stage0623Copy");
            Debug.Log("테스트 신 시작");       
        }
      
        public void SettingPanelOn()
        {
            OpenPanel(SettingPanel);
        }

        public void SaveSlotPanelOn()
        {
            SaveSlotsInit();       
            OpenPanel(SaveSlotPanel);      
        }

        public void SaveSlotSelected(int slotIndex)
        {
            saveFileNameCashing = $"save_slot_{slotIndex + 1}.json";
            OpenPanel(ConformPanel);
        }

        public void ConfirmSlotCancel()
        {
            ClosePanel();       
        }

        public void ConfirmSlotConfirmed()
        {
            CloseAllPanels();
            Debug.Log($"{saveFileNameCashing} 선택됨");
            // 저장 방식에 따라서 각 세이브파일을 로드하는 방식 변경
            // LoadGame(saveFileNameCashing);       
        }
        public void ExitGame()
        {
            CloseAllPanels();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
        }
    #endregion
    }
}