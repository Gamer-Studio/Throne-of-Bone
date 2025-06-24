using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ToB.Scenes.Intro
{
    public class IntroUI : MonoBehaviour
    {
        [SerializeField] public GameObject MainPanel;
        [SerializeField] public GameObject SaveSlotPanel;
        [SerializeField] public GameObject SettingPanel;
        
        [Header("Setting Panel")]
        
        [Header("Save Slot Panel")]
        [SerializeField] public GameObject ConformPanel;
        
        private void Awake()
        {
            UIManager.Instance.Init(this);            
        }
        
        public void StartGame()
        {
            SceneManager.LoadScene("Stage");
            Debug.Log("Start");
        }
        
        public void LoadGame()
        {
            SceneManager.LoadScene("Stage");
            Debug.Log("Load");       
        }

        public void MainPanelOn()
        {
            MainPanel.SetActive(true);
            SaveSlotPanel.SetActive(false);
            SettingPanel.SetActive(false);
        }
        
        public void SettingPanelOn()
        {
            MainPanel.SetActive(false);
            SaveSlotPanel.SetActive(false);
            SettingPanel.SetActive(true);
        }

        public void SaveSlotPanelOn()
        {
            SaveSlotPanel.SetActive(true);
            MainPanel.SetActive(false);
            SettingPanel.SetActive(false);       
        }

        public void SaveSlotSelected()
        {
            ConformPanel.SetActive(true);
        }

        public void SaveSlotCancel()
        {
            ConformPanel.SetActive(false);       
        }

        public void SaveSlotConfirmed()
        {
            ConformPanel.SetActive(false);
            SaveSlotPanel.SetActive(false);
            // 저장 방식에 따라서 각 세이브파일을 로드하는 방식 변경 예정
            StartGame();       
        }

        public void LoadButtonClick()
        {
            MainPanel.SetActive(false);
            SaveSlotPanel.SetActive(true);
        }

        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
        }

        public void SettingButtonClick()
        {
            Debug.Log("Option");
        }

    }
}