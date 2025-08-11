using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ToB.Core;
using ToB.UI.WideMap;
using ToB.Utils.Singletons;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AudioType = ToB.Core.AudioType;

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
        public ToastUI toastUI;

        public Image fadePanel;

        [SerializeField] private EventSystem eventSystem;
        
        public readonly Stack<UIPanelBase> panelStack = new Stack<UIPanelBase>();
        
        #endregion

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            eventSystem.enabled = true;
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
                    toastUI.gameObject.SetActive(false);
                    AudioManager.Play("0.Title",AudioType.Background);
                    break;
                
                case "Intro":
                    introUI.gameObject.SetActive(false);
                    crossHairUI.gameObject.SetActive(false);
                    gamePlayUI.gameObject.SetActive(false);
                    mainBookUI.gameObject.SetActive(false);
                    wideMapUI.gameObject.SetActive(false);
                    gameOverUI.gameObject.SetActive(false);
                    effectUI.gameObject.SetActive(false);
                    toastUI.gameObject.SetActive(false);
                    break;
        
                case Defines.StageIntroScene:
                    introUI.gameObject.SetActive(false);
                    crossHairUI.gameObject.SetActive(false);
                    gamePlayUI.gameObject.SetActive(false);
                    mainBookUI.gameObject.SetActive(false);
                    wideMapUI.gameObject.SetActive(false);
                    gameOverUI.gameObject.SetActive(false);
                    effectUI.gameObject.SetActive(false);
                    toastUI.gameObject.SetActive(false);
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
                    toastUI.gameObject.SetActive(false);
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

        public void InitsAfterStageAwake()
        {
            gameOverUI.Init();
        }
        
        #region FadeFX

        public IEnumerator FadeIn(float time)
        {
            Tween t = fadePanel.DOFade(0, time);
            yield return new WaitUntil(() => !t.IsActive() || t.IsComplete());
        }
        public IEnumerator FadeOut(float time)
        {
            Tween t = fadePanel.DOFade(1, time);
            yield return new WaitUntil(() => !t.IsActive() || t.IsComplete());
            
        }
        
        
        #endregion
    }
}