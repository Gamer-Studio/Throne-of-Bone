using System.Collections;
using ToB.Utils.Singletons;
using UnityEngine.SceneManagement;

namespace ToB.UI
{
    public enum SceneName
    {
        Invaild = -1,
        Intro = 0,
        MainMenu = 1,
        GamePlay = 2,
        LoadingScene = 99
    }
    // 추후 dev branch에서 수정 요망 (IntroScene, MenuScene, StageScene, LoadingScene, ...)
    // SceneManager랑 잇기 편하게 Dictionary화 할지도 생각 중인데, 신이 4개밖에 없어서 그냥 하드코딩해도 될 것 같기도...

    public class UIManager : DDOLSingleton<UIManager>
    {
        #region InitPanels
        
        public IntroUI introUI;
        public CrossHairUI crossHairUI;
        public GamePlayUI gamePlayUI;

        public MainBookUI mainBookUI;
        public WideMapUI wideMapUI;
        public GameOverUI gameOverUI;
        //[SerializeField] public ToastUI toastUI;

        public void Init(IntroUI _introUI)
        {
            this.introUI = _introUI;
        }

        public void Init(CrossHairUI _crossHairUI)
        {
            this.crossHairUI = _crossHairUI;
        }

        public void Init(GamePlayUI _gamePlayUI)
        {
            this.gamePlayUI = _gamePlayUI;
        }
        
        public void Init(MainBookUI _mainBookUI)
        {
            this.mainBookUI = _mainBookUI;
        }
        
        public void Init(WideMapUI _wideMapUI)
        {
            this.wideMapUI = _wideMapUI;
        }
        
        public void Init(GameOverUI _gameOverUI)
        {
            this.gameOverUI = _gameOverUI;
        }
        
        /*
        public void Init(ToastUI _toastUI)
        {
            this.toastUI = _toastUI;
        }
        */
        
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
            yield return null;
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
                    break;
                
                case "Intro":
                    introUI.gameObject.SetActive(false);
                    crossHairUI.gameObject.SetActive(false);
                    gamePlayUI.gameObject.SetActive(false);
                    mainBookUI.gameObject.SetActive(false);
                    wideMapUI.gameObject.SetActive(false);
                    gameOverUI.gameObject.SetActive(false);
                    break;
                
                case "Stage0623Copy":
                    introUI.gameObject.SetActive(false);
                    crossHairUI.gameObject.SetActive(true);
                    gamePlayUI.gameObject.SetActive(true);
                    mainBookUI.gameObject.SetActive(false);
                    wideMapUI.gameObject.SetActive(false);
                    gameOverUI.gameObject.SetActive(false);
                    break;
                
                default:
                    introUI.gameObject.SetActive(false);
                    crossHairUI.gameObject.SetActive(true);
                    gamePlayUI.gameObject.SetActive(true);
                    mainBookUI.gameObject.SetActive(false);
                    wideMapUI.gameObject.SetActive(false);
                    gameOverUI.gameObject.SetActive(false);
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

        /*
         public void CancelUI(InputAction.CallbackContext context)
         
        {
            Debug.Log("UIManager - ESC 인식");
            CloseActiveUI();
        }

        private void CloseActiveUI()
        {
            if (IsThereOverlayUI())
            {
                wideMapUI.gameObject.SetActive(false);
                mainBookUI.gameObject.SetActive(false);
            }
        }
        */
        #endregion
            



    }
}