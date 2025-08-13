using System;
using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using ToB.Core;
using ToB.Entities.Buffs;
using ToB.World.Structures;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace ToB.Scenes.Intro
{
  public class ContentLoader : MonoBehaviour
  {
    
    public static bool isLoaded { get; private set; } = false;
    
    [SerializeField] private TMP_Text loadingText;
    private Tween loadingTween;
    private bool loadStageIsNew;

    private async UniTask LoadContent()
    {
      if(isLoaded) return;
      
      var (sharedTableLoader, stringTableHandle) = Localizer.Load();

      (string name, AsyncOperationHandle loader)[] loaderList = {
        ("Shared Table", sharedTableLoader),
        ("언어 번들", stringTableHandle),
        ("버프 데이터", Buff.Load()),
        ("생성형 오브젝트", Structure.Load())
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

      await WaitForLoadAll(loaderList);
      
      isLoaded = true;
    }

    private async UniTask WaitForLoadAll( (string name, AsyncOperationHandle loader)[] loaderList)
    {
      foreach (var operation in loaderList)
      {
        await operation.loader.ToUniTask();
      }
    } 
    IEnumerator Start()
    {

      loadingText.alpha = 0;

      yield return null;
      loadingTween = loadingText.DOFade(1, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
      yield return LoadContent().ToCoroutine();
    }

    private void Update()
    {
      if (isLoaded) 
      {
        StartCoroutine(LoadSceneAfterTween());
      }
    }

    IEnumerator LoadSceneAfterTween()
    {
      loadingTween.Kill();
      loadingTween = loadingText.DOFade(0, 0.5f);
      yield return new WaitForSeconds(0.5f);
      loadingTween.Kill();
      SceneManager.LoadScene(Defines.MainMenuScene);
    }
    
  }
  
}