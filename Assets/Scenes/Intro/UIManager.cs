using ToB.Utils.Singletons;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ToB.Scenes.Intro
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
        [SerializeField] public IntroUI introUI;
        [SerializeField] public CrossHairUI crossHairUI;
        [SerializeField] public GamePlayUI gamePlayUI;

        [SerializeField] public MainBookUI mainBookUI;
        [SerializeField] public WideMapUI wideMapUI;
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
        
        /*
        public void Init(ToastUI _toastUI)
        {
            this.toastUI = _toastUI;
        }
        */
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
       
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            switch (SceneManager.GetActiveScene().name)
            {
                case "MainMenu":
                    introUI.gameObject.SetActive(true);
                    crossHairUI.gameObject.SetActive(true);
                    gamePlayUI.gameObject.SetActive(false);
                    mainBookUI.gameObject.SetActive(false);
                    wideMapUI.gameObject.SetActive(false);
                    break;
                
                case "Stage":
                    introUI.gameObject.SetActive(false);
                    crossHairUI.gameObject.SetActive(true);
                    gamePlayUI.gameObject.SetActive(true);
                    mainBookUI.gameObject.SetActive(false);
                    wideMapUI.gameObject.SetActive(false);
                    break;
                
                default:
                    introUI.gameObject.SetActive(false);
                    crossHairUI.gameObject.SetActive(false);
                    gamePlayUI.gameObject.SetActive(false);
                    mainBookUI.gameObject.SetActive(false);
                    wideMapUI.gameObject.SetActive(false);
                    break;
            }
        }
        



    }
}