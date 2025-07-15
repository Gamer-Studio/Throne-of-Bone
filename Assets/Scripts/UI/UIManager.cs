using System.Collections;
using System.Collections.Generic;
using ToB.Utils.Singletons;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ToB.UI
{
    public class UIManager : DDOLSingleton<UIManager>
    {
        #region InitPanels
        
        public IntroUI introUI;
        public CrossHairUI crossHairUI;
        public GamePlayUI gamePlayUI;

        public MainBookUI mainBookUI;
        public WideMapUI wideMapUI;
        public GameOverUI gameOverUI;
        public EffectUI effectUI;
        //[SerializeField] public ToastUI toastUI;
        
        public readonly Stack<UIPanelBase> panelStack = new Stack<UIPanelBase>();
        
        #endregion

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private IEnumerator WaitForInits()
        {
            yield return new WaitForSeconds(0.2f);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StartCoroutine(WaitForInits());
            
            switch (SceneManager.GetActiveScene().name)
            {
                case "MainMenu":
                    introUI.gameObject.SetActive(true);
                    crossHairUI.gameObject.SetActive(true);
                    gamePlayUI.gameObject.SetActive(false);
                    mainBookUI.gameObject.SetActive(false);
                    wideMapUI.gameObject.SetActive(false);
                    gameOverUI.gameObject.SetActive(false);
                    effectUI.gameObject.SetActive(false);
                    break;
                
                case "Intro":
                    introUI.gameObject.SetActive(false);
                    crossHairUI.gameObject.SetActive(false);
                    gamePlayUI.gameObject.SetActive(false);
                    mainBookUI.gameObject.SetActive(false);
                    wideMapUI.gameObject.SetActive(false);
                    gameOverUI.gameObject.SetActive(false);
                    effectUI.gameObject.SetActive(false);
                    break;
        
                //메인메뉴와 인트로 씬 말고는 다 Stage씬이니 일단은 이렇게.
                //TODO: 추후 로딩씬이 생길 경우 케이스 추가할 필요 있음
                default:
                    introUI.gameObject.SetActive(false);
                    crossHairUI.gameObject.SetActive(true);
                    gamePlayUI.gameObject.SetActive(true);
                    mainBookUI.gameObject.SetActive(false);
                    wideMapUI.gameObject.SetActive(false);
                    gameOverUI.gameObject.SetActive(false);
                    effectUI.gameObject.SetActive(true);
                    break;
            }
        }
        
        #region PlayerInput
        public bool isThereActiveUI = false;
        public bool IsThereOverlayUI()
        {
            if (mainBookUI.gameObject.activeSelf || wideMapUI.gameObject.activeSelf)
            {
                return isThereActiveUI = true;
            }
            else
            {
                return isThereActiveUI = false;
            }
        }
        #endregion
            



    }
}