using System.Linq;
using TMPro;
using ToB.Core;
using ToB.Worlds;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace ToB.Scenes.Intro
{
  public class ContentLoader : MonoBehaviour
  {
    /*
    public static bool isLoaded { get; private set; } = false;
    
    [SerializeField] private TMP_Text loadingText;

    private void LoadContent()
    {
      if(isLoaded) return;
      
      var (sharedTableLoader, stringTableHandle) = Localizer.Load();
      var (mixerHandle, clipHandle ) = AudioManager.Load();

      (string name, AsyncOperationHandle loader)[] loaderList = {
        ("Shared Table", sharedTableLoader),
        ("언어 번들", stringTableHandle),
        ("소리 설정", mixerHandle),
        ("음원", clipHandle),
      };

      foreach (var operation in loaderList)
      {
        operation.loader.Completed += _ =>
        {
          var temp = (from op in loaderList
            where !op.loader.IsDone
            orderby op.loader.PercentComplete descending
            select op);

          if (temp.Any())
          {
            var next = temp.First();
            loadingText.text = "loading " + next.name + " ...";
          }
        };
      }

      foreach (var operation in loaderList)
      {
        operation.loader.WaitForCompletion();
      }
      
      isLoaded = true;
    }
    
    private void Start()
    {
      LoadContent();

      SceneManager.LoadScene("MainMenu");
    }
    */
  }
  
}