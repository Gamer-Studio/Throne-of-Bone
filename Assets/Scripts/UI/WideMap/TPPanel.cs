using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using ToB.Core;
using ToB.IO;
using ToB.IO.SubModules.SavePoint;
using UnityEngine;
using UnityEngine.Localization.SmartFormat;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ToB.UI.WideMap
{
  public class TPPanel : MonoBehaviour
  {
    #region State

    [SerializeField, ReadOnly] private List<Button> buttons = new();

    #endregion
    
    #region Bindings

    private const string Binding = "Binding";

    [Foldout(Binding), SerializeField] private Transform buttonContainer;
    [Foldout(Binding), SerializeField] private Button originButton;

    #endregion

    private void OnEnable()
    {
      if(SAVE.Current == null || SAVE.Current.IsLoaded) return;
      
      foreach (var button in buttons.Where(button => button))
        Destroy(button.gameObject);

      buttons.Clear();
      
      var module = SAVE.Current.SavePoints;

      var currentPoint = module.GetLastSavePoint();
      var savePoints = module.activeSavePoints;

      foreach (var data in savePoints)
      {
        var button = Instantiate(originButton, buttonContainer);

        button.GetComponentInChildren<TMP_Text>().text =
          "TPPointButton".Localize("Global").FormatSmart(data.stageIndex, data.roomIndex);

        if(data != currentPoint) button.onClick.AddListener(() => data.Teleport());
        else
        {
          button.interactable = false;
          button.GetComponent<Image>().color = Color.gray;
        }
        
        button.gameObject.SetActive(true);
      }
    }
  }
}