using System.Linq;
using TMPro;
using ToB.Core;
using ToB.Entities.Buffs;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ToB.Scenes.Intro
{
  public class ContentLoader : MonoBehaviour
  {
    
    public static bool isLoaded { get; private set; } = false;
    
    [SerializeField] private TMP_Text loadingText;

    private void LoadContent()
    {
      if(isLoaded) return;
      
      var (sharedTableLoader, stringTableHandle) = Localizer.Load();

      (string name, AsyncOperationHandle loader)[] loaderList = {
        ("Shared Table", sharedTableLoader),
        ("언어 번들", stringTableHandle),
        ("버프 데이터", Buff.Load())
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
            //loadingText.text = "loading " + next.name + " ...";
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

      //SceneManager.LoadScene(Defines.MainMenuScene);
    }
    
  }
  
}