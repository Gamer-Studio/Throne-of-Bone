using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ToB.Scenes.Intro
{
    public class IntroUI : MonoBehaviour
    {
        [SerializeField] public Button StartButton;
        [SerializeField] public Button ExitButton;
        [SerializeField] public Button OptionsButton;
        [SerializeField] public Button LoadButton;
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
    
        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
        }

        public void OptionButton()
        {
            Debug.Log("Option");
        }

    }
}