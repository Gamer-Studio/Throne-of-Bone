using TMPro;
using ToB.IO;
using UnityEngine;

namespace ToB.UI
{
  public class SaveSlot : MonoBehaviour
  {
    public SAVE save;
    [SerializeField] private TMP_Text infoText;

    public void SetSave(SAVE save, int index)
    {
      if (save.name != "empty")
        infoText.text = $"세이브 슬롯 {index + 1} - {save.name}\n날짜 : {save.SaveTime}\n보유 골드 : {save.gold}";
      else
        infoText.text = $"세이브 슬롯 {index + 1} - EMPTY";
    }
  }
}