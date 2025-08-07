using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using ToB.Core;
using ToB.IO;
using ToB.Utils;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace ToB.UI
{
    public class IntroUI : UIPanelBase
    {
        [Scene] public string stageSceneName;
        [SerializeField] public GameObject MainPanel;
        [SerializeField] public GameObject SaveSlotPanel;
        [SerializeField] public GameObject SettingPanel;
        
        
        [SerializeField] public CanvasGroup introUIGroup;
        
        [Header("Setting Panel")]
        
        [Header("Save Slot Panel")]
        [Foldout("Save Slot Panel"), SerializeField] public GameObject ConformPanel;
        [Foldout("Save Slot Panel"), SerializeField] public GameObject DeleteConfirmPanel;
        [Foldout("Save Slot Panel"), SerializeField] public Button[] saveSlotButtons;
        [Foldout("Save Slot Panel"), SerializeField] public Button[] deleteSlotButtons;
        [Label("로딩된 세이브파일 목록"), Foldout("Save Slot Panel"), SerializeField] private SAVE[] saves;
        [Label("선택된 세이브파일"), Foldout("Save Slot Panel"), SerializeField] private SAVE selectedSave;

        void OnEnable()
        {
            MainPanel.SetActive(true);
            SaveSlotPanel.SetActive(false);
            SettingPanel.SetActive(false);
            
            StartCoroutine(FadeCoroutine());
        }

        IEnumerator FadeCoroutine()
        {
            introUIGroup.alpha = 0;
            yield return null;
            introUIGroup.DOFade(1, 1f);
        }

        private async Task SaveSlotsInit()
        {
            saves = await SAVE.GetAllSaves();
            
            for (var i = 0; i < saveSlotButtons.Length; i++)
            {
                var save = saves[i];
                var index = i;
                
                saveSlotButtons[i].onClick.AddListener(() => SaveSlotSelected(index));
                
                var textField = saveSlotButtons[i].GetComponentInChildren<TMP_Text>();
                
                if (save.name != "empty")
                {
                    // 빈 슬롯이 아닐 때
                    textField.text = $"세이브 슬롯 {i + 1} - {save.name}\n날짜 : {save.SaveTime}";
                    deleteSlotButtons[i].onClick.AddListener(() => DeleteSaveFileSelected(index));
                    deleteSlotButtons[i].gameObject.SetActive(true);
                }
                else
                {
                    // 빈 슬롯일 때
                    textField.text = $"세이브 슬롯 {i + 1} - EMPTY";
                    deleteSlotButtons[i].gameObject.SetActive(false);
                }
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
            SceneManager.LoadScene(Defines.MainMenuScene);
            Debug.Log("메인 메뉴");
        }
        
        public void StartGame()
        {
            CloseAllPanels();
            SceneManager.LoadScene(stageSceneName);
            Debug.Log("게임 시작");
        }
        
        public void LoadGame()
        {
            CloseAllPanels();
            if (SAVE.Current.isFirstEnter) SceneManager.LoadScene(Defines.StageIntroScene);
            else SceneManager.LoadScene(stageSceneName);
            Debug.Log("테스트 신 시작");       
        }
      
        public void SettingPanelOn()
        {
            OpenPanel(SettingPanel);
        }


        public async void SaveSlotPanelOn()
        {
            try
            {
                await SaveSlotsInit();       
                OpenPanel(SaveSlotPanel);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void SaveSlotSelected(int selected)
        {
            OpenPanel(ConformPanel);
            selectedSave = saves[selected];
        }

        public void DeleteSaveFileSelected(int selected)
        {
            OpenPanel(DeleteConfirmPanel);
            selectedSave = saves[selected];
            selectedSave.Delete();
            _ = SaveSlotsInit();
        }

        public void DeleteSaveFileConfirmed()
        {
            ClosePanel();
            UIManager.Instance.toastUI.Show("선택한 세이브 파일을 삭제했습니다.");
            // ~이하 세이브 파일 삭제 로직~
        }

        public void ConfirmSlotCancel()
        {
            ClosePanel();       
        }

        public async void ConfirmSlotConfirmed()
        {
            try
            {
                await selectedSave.LoadAll();
                // 저장 방식에 따라서 각 세이브파일을 로드하는 방식 변경 예정
                LoadGame();
                CloseAllPanels();
            }
            catch (Exception e)
            {
                DebugSymbol.UI.Log(e);
                // ignored
            }
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

        public override void Process(InputAction.CallbackContext context)
        {
            
        }

        public override void Cancel(InputAction.CallbackContext context)
        {
            CloseCurrentPanel(context);
        }
    }
}