using UnityEngine;
using UnityEngine.SceneManagement;

namespace ToB.Scenes.MainMenu
{
  public class MainMenuSystem : MonoBehaviour
  {
    public void StartGame()
    {
      SceneManager.LoadScene("Stage");
    }
    
    public void ExitGame()
    {
      #if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
      #else
      Application.Quit();
      #endif
    }
  }
}