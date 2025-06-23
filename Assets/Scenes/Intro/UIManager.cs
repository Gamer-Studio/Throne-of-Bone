using ToB.Utils.Singletons;
using UnityEngine;

namespace ToB.Scenes.Intro
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] public IntroUI introUI;
        //[SerializeField] public GameplayUI gameplayUI;
        //[SerializeField] public MainUI mainUI;
        //[SerializeField] public ToastUI toastUI;

        public void Init(IntroUI _introUI)
        {
            this.introUI = _introUI;
        }
        
        /*
        public void Init(GameplayUI _gameplayUI)
        {
            this.gameplayUI = _gameplayUI;
        }
        
        public void Init(MainUI _mainUI)
        {
            this.mainUI = _mainUI;
        }
        
        public void Init(ToastUI _toastUI)
        {
            this.toastUI = _toastUI;
        }
        */
        
        



    }
}