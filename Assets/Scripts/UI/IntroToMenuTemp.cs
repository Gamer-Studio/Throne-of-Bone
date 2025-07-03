using UnityEngine;
using UnityEngine.SceneManagement;

namespace ToB.UI
{
    public class IntroToMenuTemp : MonoBehaviour
    {
        public void ToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
        
    }
}