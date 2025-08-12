using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using ToB.Core;
using ToB.IO;
using ToB.IO.SubModules.SavePoint;
using ToB.Scenes.Stage;
using UnityEngine;
using UnityEngine.Localization.SmartFormat;
using UnityEngine.UI;

namespace ToB.UI.WideMap
{
  public class TPPanel : MonoBehaviour
  {
    #region State

    [SerializeField, ReadOnly] private List<Button> buttons = new();

    #endregion

    private void OnEnable()
    {
      if (SAVE.Current == null || !SAVE.Current.IsLoaded) return;
      foreach (var button in buttons) Destroy(button.gameObject);

      buttons.Clear();

      var module = SAVE.Current.SavePoints;

      var currentPoint = module.GetLastSavePoint();
      var savePoints = module.activeSavePoints;

      for (var i = 0; i < savePoints.Count; i++)
      {
        var data = savePoints[i];
        // Debug.Log($"load save point {data.stageIndex} {data.roomIndex}");
        var button = Instantiate(originButton, buttonContainer);

        button.GetComponentInChildren<TMP_Text>().text =
          "TPPointButton".Localize("Global").FormatSmart(data.stageIndex, data.roomIndex);

        if (data != currentPoint)
        {
          var i1 = i;
          button.onClick.AddListener(() => Teleport(data, i1));
        }
        else
        {
          button.interactable = false;
          button.GetComponent<Image>().color = Color.gray;
        }

        button.gameObject.SetActive(true);
        buttons.Add(button);
      }
    }

    private void Teleport(SavePointData data, int pointIndex)
    {
      data.Teleport();
      transform.parent?.gameObject.SetActive(false);
      StageManager.Instance.ChangeGameState(GameState.Play);
    }

    #region Bindings

    private const string Binding = "Binding";

    [Foldout(Binding)] [SerializeField] private Transform buttonContainer;
    [Foldout(Binding)] [SerializeField] private Button originButton;

    #endregion
  }
}