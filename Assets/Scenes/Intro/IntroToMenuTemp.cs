using UnityEngine;
using UnityEngine.SceneManagement;

namespace ToB.Scenes.Intro
{
    public class IntroToMenuTemp : MonoBehaviour
    {
        public void ToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
        
    }
}